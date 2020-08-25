using System;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Text;

namespace WinClipboardLibrary
{
    public class ClipData
    {
        public int groupID = 0;
        public string strText = "";
        public Image img = null;
        public int ClipDataType = 0;
        public int nDataLen = 0;

        public byte[] data;

        public string GetText()
        {
            return strText;
        }
        public Image GetImage()
        {
            return img;
        }
        public int ClipboardDataType()
        {
            return ClipDataType;
        }

    }
    public partial class WinClipDllForm : Form
    {
        public delegate void RecvClipBoardEvent(int nGroupId, int nType, int nLength, byte[] pMem);

        public event RecvClipBoardEvent SGClipEvent;

        public string strText = "";
        public Image img = null;
        ClipData clipData;
        public WinClipDllForm()
        {
            InitializeComponent();
        }

        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, KeyModifiers fsModifiers, Keys vk);

        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        const int HOTKEY_ID = 31197; //Any number to use to identify the hotkey instance

        public enum KeyModifiers
        {
            None = 0,
            Alt = 1,
            Control = 2,
            Shift = 4,
            Windows = 8
        }
        const int WM_HOTKEY = 0x0312;
        protected override void WndProc(ref Message message)
        {
            switch (message.Msg)
            {
                case WM_HOTKEY:
                    int groupID = -1;
                    for(int i=0;i<10;i++)
                    {
                        if ( ((int)(message.WParam)) == (HOTKEY_ID+i))
                        {
                            Keys key = (Keys)(((int)message.LParam >> 16) & 0xFFFF);
                            KeyModifiers modifier = (KeyModifiers)((int)message.LParam & 0xFFFF);
                            groupID = i;
                            //MessageBox.Show("HotKey Pressed :" + modifier.ToString() + " " + key.ToString());
                            //MessageBox.Show("GroupID :" + groupID.ToString());
                        }
                    }

                    GetClipboardData(groupID);
                    break;
            }
            base.WndProc(ref message);
        }
        public void RegHotKey(int groupID, bool bAlt, bool bControl, bool bShift, bool bWin, char chVKCode)
        {
            int regID = HOTKEY_ID + groupID;
            KeyModifiers fsModifiers = KeyModifiers.None;
            if (bAlt)
                fsModifiers |= KeyModifiers.Alt;             // Alt 키 조합 (0x0001)
            if (bControl)
                fsModifiers |= KeyModifiers.Control;         // Control 키 조합 (0x0002)
            if (bShift)
                fsModifiers |= KeyModifiers.Shift;           // Shift 키 조합 (0x0004)
            if (bWin)
                fsModifiers |= KeyModifiers.Windows;			// Window 키 조합 (0x0008)

            Keys key = (Keys)chVKCode;
            RegisterHotKey(this.Handle, regID, fsModifiers, key);
        }

        public void UnRegHotKey(int groupID)
        {
            UnregisterHotKey(this.Handle, HOTKEY_ID);
        }
        public void GetClipboardData(int groupID)
        {
            clipData = new ClipData();
            IDataObject iData = Clipboard.GetDataObject();
            if(iData.GetDataPresent(DataFormats.Text))                          // Text 일 경우
            {
                clipData.strText = Clipboard.GetText();
                clipData.ClipDataType = 0;
                clipData.data = Encoding.Default.GetBytes(clipData.strText);
                clipData.nDataLen = clipData.data.Length;
            }

            else if(iData.GetDataPresent(DataFormats.Bitmap))                   // Image 일 경우
            {
                clipData.img = Clipboard.GetImage();
                clipData.ClipDataType = 1;
                clipData.data = converterDemo(clipData.img);
                clipData.nDataLen = clipData.data.Length;
            }
            
            else
            {
                
            }

            SGClipEvent(groupID, clipData.ClipDataType, clipData.nDataLen, clipData.data);

        }

        public void SetRecvClipEvent(RecvClipBoardEvent rClipBoardEvent)
        {
            SGClipEvent = rClipBoardEvent;
        }
        public byte[] converterDemo(Image x)
        {
            ImageConverter _imageConverter = new ImageConverter();
            byte[] xByte = (byte[])_imageConverter.ConvertTo(x, typeof(byte[]));
            return xByte;
        }

        public Image byteArrayToImage(byte[] byteArrayIn)
        {
            try
            {
                MemoryStream ms = new MemoryStream(byteArrayIn, 0, byteArrayIn.Length);
                ms.Write(byteArrayIn, 0, byteArrayIn.Length);
                img = Image.FromStream(ms, true);
                    
            }
            catch(Exception e)
            {
                return null;
            }
            return img;
        }

        public void SetClipboardData(int Type, byte[] clipData)
        {
            if (Type == 0)
            {
                string strText = Encoding.Default.GetString(clipData);
                Clipboard.SetText(strText);
            }

            else if(Type==1)
            {
                img = byteArrayToImage(clipData);
                if (img != null)
                    Clipboard.SetImage(img);
            }
        }

        public void ClipboardClear()
        {
            Clipboard.Clear();
        }

        public void SetImageTest(string strImagePath)
        {
            Image image = Image.FromFile(strImagePath);
            Clipboard.SetImage(image);
        }
    }
}
