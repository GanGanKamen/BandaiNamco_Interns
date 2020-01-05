@echo off
setlocal
set server_root=\\172.28.35.250
set server_path=%server_root%\Intern2019\programmer

cd /d %~dp0

:begin
echo.
set /p tank="コピーする戦車番号を入力してください (1-22) : "
set tank=00%tank%
set dirname=TankAI-%tank:~-2,2%

if exist Assets\Scripts\TankAI\%dirname% goto alreadyExists
@mklink /D Assets\Scripts\TankAI\%dirname% %server_path%\Tanks\%dirname%
if errorlevel 1 goto error
xcopy /Q /I /Y %server_path%\Tanks\TankAI-00 Assets\Scripts\TankAI\TankAI-Sample

echo %dirname% を作りました。
goto end

:alreadyExists
echo.
echo %dirname% はすでに存在しています。
echo 終了したい場合はこのウィンドウを閉じてください。
pause
goto begin

:error
echo.
echo 失敗しました。管理者権限で実行してください。
echo (このバッチファイルを右クリックして【管理者として実行】をクリック)

:end
echo.
pause

