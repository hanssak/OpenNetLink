
# 컨트리뷰터 가이드

GitHub를 통해 협업을 처음한다면 어색하거나 어려운 부분이 있을 수 있지만 편안하게 임할수록 더 잘 배울 수 있습니다. 

첫 번째 기여를 하려면 그저 아래의 간단한 단계를 따르면 됩니다.

<img align="right" width="300" src="../content/fork.png" alt="이 저장소 포크하기" />

지금 Git이 없으면 [설치](https://help.github.com/articles/set-up-git/)하시기 바랍니다.

## 저장소 Fork하기

이 페이지의 위에 있는 Fork 버튼을 클릭하여 이 저장소를 Fork하세요. 그러면 자신의 계정에 이 저장소의 복제본이 생성될 겁니다.

## 저장소 복제하기

<img align="right" width="300" src="../content/clone.png" alt="이 저장소 복제하기" />

이제 이 저장소를 자신의 컴퓨터에 복제합니다. 복제 버튼을 클릭하고 클립보드로 복사 아이콘을 클릭합니다.

터미널을 열고 다음 Git 명령을 실행합니다:

```
git clone "방금 복사한 주소"
```

(따옴표를 제외한) "방금 복사한 주소"는 이 저장소의 주소입니다. 주소를 얻으려면 이전 단계를 참조하세요.

<img align="right" width="300" src="../content/copy-to-clipboard.png" alt="URL 을 클립보드로 복사" />

예시:

```
git clone https://github.com/your_id/OpenNetLink.git
```

`your_id`는 당신의 깃허브 계정입니다. 여기서 깃허브에 있는
OneNetLink 저장소의 내용을 컴퓨터에 복사합니다.

## 브랜치 생성하기

아직 저장소 디렉토리에 있지 않다면 그곳으로 이동합니다.

```
cd OpenNetLink
```

이제 `git checkout` 명령을 사용하여 브랜치를 생성합니다.

```
git checkout -b <add_your_id>
```

예시:

```
git checkout -b add_johndoe
```

(브랜치의 이름에 꼭 *add*가 들어가지 않아도 됩니다. 하지만 이 브랜치의 목적은 당신의 이름을 리스트에 추가하는 것이기 때문에 이름에 *add*를 포함하는 것이 타당합니다.)

## 필요한 변경사항을 작성하고 커밋하기

이제 텍스트 편집기에서 `CONTRIBUTORS.md` 파일을 열어서 편집을 합니다.
Markdown을 어떻게 사용하는지는 이 [치트시트](https://github.com/adam-p/markdown-here/wiki/Markdown-Cheatsheet)를 참조하세요.

`CONTRIBUTORS.md`의 마지막에 하기의 정보를 추가하세요.

```
- [your-name](https://github.com/your_id)
```

예시:

```
- [John Doe](https://github.com/johndoe)
```

`](` 사이에 스페이스가 없다는 것에 주의하시기 바랍니다. 파일을 저장하고 종료하세요.

프로젝트 디렉터리에서 `git status` 명령을 실행하면 변경사항을 볼 수 있습니다. 변경사항을 아래 `git add` 명령으로 추가합니다.

```
git add CONTRIBUTORS.md
```

이제 아래 `git commit` 명령으로 변경사항을 커밋합니다.

```
git commit -m "Add <your_id> to Contributors list"
```

`<your_id>`을 자신의 github 계정으로 바꾸세요.

## 변경사항을 깃허브에 푸시하기

`git push` 명령으로 변경사항을 푸시합니다.

```
git push origin <add_your_id>
```

`<add-your-name>`을 이전에 생성한 브랜치 이름으로 바꾸세요.

## 검토를 위해 변경사항을 제출하기

깃허브의 당신의 저장소에 가면, `Compare & pull request` 버튼을 볼 수 있습니다. 그 버튼을 클릭하세요.

<img style="float: right;" src="../content/compare-and-pull.png" alt="풀 요청
생성하기" />

이제 풀 요청을 제출합니다.

<img style="float: right;" src="../content/submit-pull-request.png" alt="풀 요청 제출하기"
/>

이제 여러분의 변경사항을 제가 확인 후에 마스터 브랜치에 머지 하게 되면 알림 메일을 받으실 수 있습니다.
