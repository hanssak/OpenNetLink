using OfficeExtractor.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;

namespace OfficeExtractor
{
    class AppendCheck
    {
        private static Serilog.ILogger CLog => Serilog.Log.ForContext<AppendCheck>();
        //이미지 파일군 구조에 맞지 않는 파일 Detect
        public int FileAppendCheck(string inputFile)
        {
            int result = 0;

            string extension = GetExtension(inputFile);

            try
            {
                #region
                switch (extension)
                {
                    case ".PNG":
                        {
                            using (FileStream stream = File.OpenRead(inputFile))
                            {
                                //파일 시그니쳐 건너띄기
                                stream.Seek(8, SeekOrigin.Begin);

                                //IHDR 건너띄기
                                stream.Seek(25, SeekOrigin.Current);

                                while (true)
                                {
                                    byte[] chunkLength = new byte[4];
                                    for (int i = 0; i < 4; i++)
                                    {
                                        chunkLength[i] = (byte)stream.ReadByte();
                                    }

                                    byte[] chunkType = new byte[4];
                                    for (int i = 0; i < 4; i++)
                                    {
                                        chunkType[i] = (byte)stream.ReadByte();
                                    }

                                    string strType = BitConverter.ToString(chunkType);
                                    //IEND를 찾아서 
                                    if (strType == "49-45-4E-44")
                                    {
                                        stream.Seek(4, SeekOrigin.Current);
                                        if (stream.Position == stream.Length)
                                        {
                                            result = 0;
                                        }
                                        else
                                        {
                                            result = -3;
                                        }
                                        break;
                                    }
                                    else
                                    {
                                        Array.Reverse(chunkLength);
                                        int length = BitConverter.ToInt32(chunkLength);

                                        if (stream.Position + length + 4 > stream.Length)
                                        {
                                            result = -4;
                                            break;
                                        }

                                        stream.Seek(length + 4, SeekOrigin.Current);
                                    }
                                }
                            }
                        }
                        break;
                    case ".BMP":
                        {
                            using (FileStream stream = File.OpenRead(inputFile))
                            {
                                //BMP의 경우 FileHeader의 2 Byte부터 거꾸로 값을 먹인다.
                                byte[] tempLength = new byte[4];
                                stream.Seek(2, SeekOrigin.Begin);

                                for (int i = 0; i < 4; i++)
                                {
                                    tempLength[i] = (byte)stream.ReadByte();
                                }

                                int length = BitConverter.ToInt32(tempLength, 0);

                                if (length == stream.Length)
                                {
                                    result = 0;
                                }
                                else
                                    result = -3;
                            }
                        }
                        break;
                    case ".JFIF":
                    case ".JPE":
                    case ".JPEG":
                    case ".JPG":
                        {
                            using (FileStream stream = File.OpenRead(inputFile))
                            {
                                stream.Seek(stream.Length - 2, SeekOrigin.Begin);
                                byte first = (byte)stream.ReadByte();
                                byte second = (byte)stream.ReadByte();

                                byte[] bLength = new byte[2];
                                stream.Seek(2, SeekOrigin.Begin);
                                string make = GetJpgMakeInfo(stream);
                                stream.Seek(2, SeekOrigin.Begin);
                                result = GetCheckJPG(stream);
                            }
                        }
                        break;
                    case ".GIF":
                        {
                            using (FileStream stream = File.OpenRead(inputFile))
                            {
                                //Global Check
                                stream.Seek(10, SeekOrigin.Begin);
                                byte[] globalTable = new byte[1];
                                globalTable[0] = (byte)stream.ReadByte();
                                stream.Seek(2, SeekOrigin.Current);
                                int colorTableLength = GetGifColorTable(globalTable);
                                if (colorTableLength > 0)
                                    stream.Seek(colorTableLength, SeekOrigin.Current);

                                while (true)
                                {
                                    byte extensionIntroucer = (byte)stream.ReadByte();

                                    if (extensionIntroucer == 0x3B)
                                    {
                                        if (stream.Position == stream.Length)
                                        {
                                            result = 0;
                                        }
                                        else
                                        {
                                            result = -3;
                                        }
                                        break;
                                    }
                                    if (!(extensionIntroucer == 0x21 || extensionIntroucer == 0x3B || extensionIntroucer == 0x2C))
                                    {
                                        result = -4;
                                        return result;
                                    }
                                    byte graphicControlLabel = (byte)stream.ReadByte();
                                    //Extension Skip
                                    if (extensionIntroucer == 0x2C)
                                    {
                                        //87a 일 경우 Image Descriptor 로만 구성
                                        stream.Seek(7, SeekOrigin.Current);
                                        byte[] localTable = new byte[1];
                                        localTable[0] = (byte)stream.ReadByte();
                                        colorTableLength = GetGifColorTable(localTable);
                                        if (colorTableLength > 0)
                                            stream.Seek(colorTableLength, SeekOrigin.Current);
                                        //LZW Minimum Code Size 건너띄기
                                        stream.Seek(1, SeekOrigin.Current);
                                        while (true)
                                        {
                                            byte subBlockSize = (byte)stream.ReadByte();
                                            if (subBlockSize == 0x00)
                                                break;
                                            int subLength = (int)subBlockSize;
                                            stream.Seek(subLength, SeekOrigin.Current);

                                        }
                                    }
                                    else if (graphicControlLabel == 0xF9)
                                    {
                                        byte extensionSize = (byte)stream.ReadByte();
                                        int extensionLength = (int)extensionSize;
                                        stream.Seek(extensionLength + 1, SeekOrigin.Current);
                                    }
                                    else if (graphicControlLabel == 0x01 || graphicControlLabel == 0xFF)
                                    {
                                        //Plain Text Extension
                                        //Application Extension
                                        byte extensionSize = (byte)stream.ReadByte();
                                        int extensionLength = (int)extensionSize;
                                        stream.Seek(extensionLength, SeekOrigin.Current);
                                        while (true)
                                        {
                                            byte size = (byte)stream.ReadByte();
                                            if (size == 0x00)
                                                break;
                                            else
                                            {
                                                int length = (int)size;
                                                stream.Seek(length, SeekOrigin.Current);
                                            }
                                        }

                                    }
                                    else if (graphicControlLabel == 0xFE)
                                    {
                                        //Comment Extension
                                        while (true)
                                        {
                                            byte size = (byte)stream.ReadByte();
                                            if (size == 0x00)
                                                break;
                                            else
                                            {
                                                int length = (int)size;
                                                stream.Seek(length, SeekOrigin.Current);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        result = -4;
                                        break;
                                    }
                                }
                            }
                        }
                        break;
                    case ".TIF":
                    case ".TIFF":
                    default:
                        result = -5;
                        break;
                }
                #endregion
            }
            catch (Exception ex)
            {
                result = -1;
                CLog.Error($"BinaryAppend Check Exception (code:{result})- {ex.ToString()}");
            }

            return result;
        }
        //파일 내부 검사를 하여 PE, MSI, ZIP 파일이 있는지 검색
        public bool FileInnerCheck(string inputFile)
        {
            bool detected = false;

            detected = GetCheckZip(inputFile);
            //ZIP 파일이 검색 되면 바로 return 
            if (detected)
                return detected;

            detected = GetCheckPe(inputFile);

            return detected;

        }
        //PE 파일 군 찾기 MSI 파일 찾기
        private static bool GetCheckPe(string inputFilePath)
        {
            bool detected = false;
            try
            {
                using (FileStream stream = File.OpenRead(inputFilePath))
                {
                    #region pattern match
                    //PE군 파일 시그니쳐
                    byte[] search = new byte[] { 0x4D, 0x5A };
                    //MSI 파일 시그니쳐
                    byte[] msiSearch = new byte[] {
                                                0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1,
                                                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                                0x3E, 0x00, 0x04, 0x00, 0xFE, 0xFF, 0x0C, 0x00
                    };
                    #endregion
                    //한번에 불러올때 읽어 오는 Byte 수
                    int length = 1024 * 1024;
                    int offset = 0;
                    int readLength = 0;
                    long maxLength = stream.Length;
                    while (true)
                    {
                        byte[] file = new byte[length];
                        if (maxLength > offset)
                        {
                            if (maxLength - offset >= length)
                            {
                                if (offset != 0)
                                    stream.Seek(offset - 32, SeekOrigin.Begin);

                                stream.Read(file, 0, length);
                                readLength = length;

                            }
                            else
                            {
                                stream.Read(file, 0, (int)maxLength - offset);
                                readLength = (int)maxLength - offset;
                            }
                        }
                        else
                            break;

                        long currentIndex = 0;
                        while (true)
                        {
                            try
                            {
                                if (file[currentIndex] == search[0])
                                {
                                    currentIndex++;
                                    if (file[currentIndex] == search[1])
                                    {
                                        currentIndex++;

                                        if ((int)maxLength <= offset + currentIndex - 2 + 4096)
                                            break;

                                        byte[] checkBinary = new byte[4096];
                                        stream.Seek(offset + currentIndex - 2, SeekOrigin.Begin);
                                        stream.Read(checkBinary, 0, 4096);
                                        stream.Seek(offset, SeekOrigin.Begin);
                                        //PE 군 검출
                                        if (GetCheckPe(checkBinary))
                                        {
                                            detected = true;
                                            break;
                                        }
                                    }
                                }
                                // MSI 파일 시그니처 검색
                                //else if (
                                //    file[currentIndex] == msiSearch[0] && file[currentIndex + 1] == msiSearch[1] && file[currentIndex + 2] == msiSearch[2] && file[currentIndex + 3] == msiSearch[3] &&
                                //    file[currentIndex + 4] == msiSearch[4] && file[currentIndex + 5] == msiSearch[5] && file[currentIndex + 6] == msiSearch[6] && file[currentIndex + 7] == msiSearch[7] &&
                                //    file[currentIndex + 8] == msiSearch[8] && file[currentIndex + 9] == msiSearch[9] && file[currentIndex + 10] == msiSearch[10] && file[currentIndex + 11] == msiSearch[11] &&
                                //    file[currentIndex + 12] == msiSearch[12] && file[currentIndex + 13] == msiSearch[13] && file[currentIndex + 14] == msiSearch[14] && file[currentIndex + 15] == msiSearch[15] &&
                                //    file[currentIndex + 16] == msiSearch[16] && file[currentIndex + 17] == msiSearch[17] && file[currentIndex + 18] == msiSearch[18] && file[currentIndex + 19] == msiSearch[19] &&
                                //    file[currentIndex + 20] == msiSearch[20] && file[currentIndex + 21] == msiSearch[21] && file[currentIndex + 22] == msiSearch[22] && file[currentIndex + 23] == msiSearch[23] &&
                                //    file[currentIndex + 24] == msiSearch[24] && file[currentIndex + 25] == msiSearch[25] && file[currentIndex + 26] == msiSearch[26] && file[currentIndex + 27] == msiSearch[27] &&
                                //    file[currentIndex + 28] == msiSearch[28] && file[currentIndex + 29] == msiSearch[29] && file[currentIndex + 30] == msiSearch[30] && file[currentIndex + 31] == msiSearch[31])
                                //{
                                //    detected = true;
                                //    break;
                                //}
                                else
                                {
                                    currentIndex++;
                                }

                                if (file.Length - 1 <= currentIndex)
                                    break;
                            }
                            catch (Exception ex)
                            {
                                return false;
                            }
                        }

                        offset += readLength;
                        readLength = 0;
                        if (detected)
                            break;
                    }

                    return detected;
                }
            }
            catch (Exception ex)
            {

                CLog.Error($"BinaryInner CheckPe Exception - {ex.ToString()}");
                return detected;
            }
        }
        private static bool GetCheckZip(string inputFilePath)
        {
            //내부 ZIP 파일, Zip Compress 로 한번 열어본다...
            bool detected = false;
            try
            {
                using (var zipFile = ZipFile.Open(inputFilePath, ZipArchiveMode.Read))
                {
                    detected = true;
                }
            }
            catch (Exception ex)
            {
                detected = false;
            }

            return detected;
        }
        private static bool GetCheckPe(byte[] stream)
        {

            //4096 만큼 읽어와 PE 인지 체크
            try
            {
                //해더 파일 검사
                byte[] search = new byte[] { 0x50, 0x45 };
                byte[] offsetByte = new byte[4];
                for (int i = 0; i < 4; i++)
                {
                    offsetByte[i] = stream[60 + i];
                }

                if (!BitConverter.IsLittleEndian)
                    Array.Reverse(offsetByte);

                int offset = BitConverter.ToInt32(offsetByte, 0);

                //// PE 시그니쳐 확인
                //if (stream[offset] != 0x50 || stream[offset + 1] != 0x45)
                //    return false;
                if (0 >= offset || offset >= 4096)
                    return false;
                if (offset + 92 > 4096)
                    return false;

                if (stream[offset] != 0x50 || stream[offset + 1] != 0x45)
                    return false;

                int indexHeaderCharacteristics = 22;
                byte[] headerCharacteristics = new byte[2];
                headerCharacteristics[0] = stream[offset + indexHeaderCharacteristics + 0];
                headerCharacteristics[1] = stream[offset + indexHeaderCharacteristics + 1];

                if (!BitConverter.IsLittleEndian)
                    Array.Reverse(headerCharacteristics);

                short flagValue = BitConverter.ToInt16(headerCharacteristics, 0);
                //IMAGE_FILE_DLL .dll
                if ((flagValue & 0x2000) == 0x2000)
                    return true;
                //IMAGE_FILE_EXECUTABLE_IMAGE .exe
                if ((flagValue & 0x0002) == 0x0002)
                    return true;

                int optionHeaderSubSystem = 92;
                byte[] subSystem = new byte[2];
                subSystem[0] = stream[offset + optionHeaderSubSystem + 0];
                subSystem[1] = stream[offset + optionHeaderSubSystem + 1];
                if (!BitConverter.IsLittleEndian)
                    Array.Reverse(subSystem);

                short subsystemValue = BitConverter.ToInt16(subSystem, 0);
                //sys
                if (subsystemValue == 2)
                    return true;

            }
            catch (Exception ex)
            {
                return false;
            }
            return false;
        }
        private static int GetCheckJPG(FileStream stream)
        {
            int result = -3;
            byte[] bLength = new byte[2];
            byte first = 0x00;
            byte second = 0x00;
            while (true)
            {
                first = (byte)stream.ReadByte();
                second = (byte)stream.ReadByte();
                if (first == 0xFF)
                {
                    //Maker 길이 가져오기
                    bLength[0] = (byte)stream.ReadByte();
                    bLength[1] = (byte)stream.ReadByte();

                    //Length 만큼 건너띈다.
                    if (BitConverter.IsLittleEndian)
                        Array.Reverse(bLength);
                    int length = (bLength[1] << 8) + bLength[0];
                    stream.Seek(length - 2, SeekOrigin.Current);

                    if (second == 0xDA)
                    {
                        break;
                    }
                }
                else
                {
                    result = -4;
                    return result;
                }
            }

            byte temp = 0x00;
            //Scan Data -> End Signal Find
            while (true)
            {
                first = (byte)stream.ReadByte();
                if (first == 0xFF)
                {
                    second = (byte)stream.ReadByte();
                    if (second == 0xD9)
                    {
                        if (stream.Position == stream.Length)
                        {
                            result = 0;
                        }
                        else
                        {
                            temp = (byte)stream.ReadByte();
                            if (temp == 0xFF)
                            {
                                temp = (byte)stream.ReadByte();
                                if (temp == 0xD8)
                                {
                                    result = GetCheckJPG(stream);
                                }
                            }
                            // 뒤가 0x00면 넘어감
                            if (result != 0)
                            {
                                while (true)
                                {
                                    temp = (byte)stream.ReadByte();
                                    if (temp == 0x00)
                                    {
                                        if (stream.Position == stream.Length)
                                        {
                                            result = 0;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        result = -3;
                                        break;
                                    }
                                }
                            }
                            //
                            if (result != 0)
                            {
                                int count = 0;

                                while (true)
                                {
                                    temp = (byte)stream.ReadByte();
                                    if (temp == 0x53)
                                    {
                                        temp = (byte)stream.ReadByte();
                                        if (temp == 0x45)
                                        {
                                            temp = (byte)stream.ReadByte();
                                            if (temp == 0x46)
                                            {
                                                temp = (byte)stream.ReadByte();
                                                if (temp == 0x54)
                                                {
                                                    if (stream.Position == stream.Length)
                                                    {
                                                        result = 0;
                                                    }
                                                    else
                                                    {
                                                        result = -3;
                                                    }
                                                    break;

                                                }
                                            }
                                        }
                                    }
                                    count++;
                                    if (count > 1000 || stream.Position == stream.Length)
                                    {
                                        result = -3;
                                        break;
                                    }
                                }
                            }

                        }
                        break;
                    }
                }
            }
            return result;
        }
        private static string GetJpgMakeInfo(FileStream stream)
        {
            string makeInfo = String.Empty;
            byte[] bLength = new byte[2];
            byte first = 0x00;
            byte second = 0x00;
            while (true)
            {
                first = (byte)stream.ReadByte();
                second = (byte)stream.ReadByte();
                if (first == 0xFF)
                {
                    //Maker 길이 가져오기
                    bLength[0] = (byte)stream.ReadByte();
                    bLength[1] = (byte)stream.ReadByte();

                    //Length 만큼 건너띈다.
                    if (BitConverter.IsLittleEndian)
                        Array.Reverse(bLength);
                    int length = (bLength[1] << 8) + bLength[0];

                    if (second == 0xE1)
                    {
                        stream.Seek(6, SeekOrigin.Current);
                        long position = stream.Position;
                        byte temp = (byte)stream.ReadByte();
                        bool isLittle = false;
                        if (temp == 0x49)
                            isLittle = true;
                        stream.Seek(3, SeekOrigin.Current);
                        byte[] bMakeLen = new byte[4];
                        for (int i = 0; i < 4; i++)
                        {
                            bMakeLen[i] = (byte)stream.ReadByte();
                        }
                        if (isLittle)
                            Array.Reverse(bMakeLen);

                        if (BitConverter.IsLittleEndian)
                            Array.Reverse(bMakeLen);

                        int templength = BitConverter.ToInt32(bMakeLen, 0);
                        stream.Seek(position + templength, SeekOrigin.Begin);
                        byte[] bCount = new byte[2];
                        bCount[0] = (byte)stream.ReadByte();
                        bCount[1] = (byte)stream.ReadByte();
                        if (isLittle)
                            Array.Reverse(bCount);

                        int count = (bCount[0] << 8) + bCount[1];

                        byte[] twoByte = new byte[2];
                        for (int i = 0; i < count; i++)
                        {
                            twoByte[0] = (byte)stream.ReadByte();
                            twoByte[1] = (byte)stream.ReadByte();
                            if (isLittle)
                                Array.Reverse(twoByte);

                            if (twoByte[0] == 0x01)
                            {
                                if (twoByte[1] == 0x0F)
                                {
                                    stream.Seek(2, SeekOrigin.Current);
                                    for (int j = 0; j < 4; j++)
                                    {
                                        bMakeLen[j] = (byte)stream.ReadByte();
                                    }
                                    if (isLittle)
                                        Array.Reverse(bMakeLen);

                                    if (BitConverter.IsLittleEndian)
                                        Array.Reverse(bMakeLen);

                                    int dataCount = BitConverter.ToInt32(bMakeLen, 0);
                                    for (int j = 0; j < 4; j++)
                                    {
                                        bMakeLen[j] = (byte)stream.ReadByte();
                                    }
                                    if (isLittle)
                                        Array.Reverse(bMakeLen);

                                    if (BitConverter.IsLittleEndian)
                                        Array.Reverse(bMakeLen);

                                    int valueOffset = BitConverter.ToInt32(bMakeLen, 0);

                                    stream.Seek(valueOffset + position, SeekOrigin.Begin);
                                    byte[] valueData = new byte[dataCount];
                                    for (int j = 0; j < dataCount; j++)
                                    {
                                        valueData[j] = (byte)stream.ReadByte();
                                    }
                                    makeInfo = BitConverter.ToString(valueData);

                                }
                            }
                        }
                        break;
                    }
                    stream.Seek(length - 2, SeekOrigin.Current);
                    if (second == 0xDA)
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }

            return makeInfo;
        }
        private static string GetExtension(string inputFile)
        {
            var extension = Path.GetExtension(inputFile);
            extension = string.IsNullOrEmpty(extension) ? string.Empty : extension.ToUpperInvariant();

            return extension;
        }
        private static int GetGifColorTable(byte[] bValue)
        {
            int result = 0;
            BitArray bit = new BitArray(bValue);
            if (bit[7])
            {
                string str = $"{Convert.ToInt32(bit[2])}{Convert.ToInt32(bit[1])}{Convert.ToInt32(bit[0])}";
                int nTable = Convert.ToInt32(str, 2);
                int length = 3 * (int)Math.Pow(2, (nTable + 1));
                result = length;
            }

            return result;
        }
    }
}
