@echo off
setlocal
set server_root=\\172.28.35.250
set server_path=%server_root%\Intern2019\programmer

cd /d %~dp0

:begin
echo.
set /p tank="�R�s�[�����Ԕԍ�����͂��Ă������� (1-22) : "
set tank=00%tank%
set dirname=TankAI-%tank:~-2,2%

if exist Assets\Scripts\TankAI\%dirname% goto alreadyExists
@mklink /D Assets\Scripts\TankAI\%dirname% %server_path%\Tanks\%dirname%
if errorlevel 1 goto error
xcopy /Q /I /Y %server_path%\Tanks\TankAI-00 Assets\Scripts\TankAI\TankAI-Sample

echo %dirname% �����܂����B
goto end

:alreadyExists
echo.
echo %dirname% �͂��łɑ��݂��Ă��܂��B
echo �I���������ꍇ�͂��̃E�B���h�E����Ă��������B
pause
goto begin

:error
echo.
echo ���s���܂����B�Ǘ��Ҍ����Ŏ��s���Ă��������B
echo (���̃o�b�`�t�@�C�����E�N���b�N���āy�Ǘ��҂Ƃ��Ď��s�z���N���b�N)

:end
echo.
pause

