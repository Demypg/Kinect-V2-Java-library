@echo off
rmdir /s /q result
rmdir /s /q final_result
if not exist final_result md final_result
set "dllName=KinectGestureLibrary"
xcopy /s \build final_result

cd bin
proxygen ..\final_result\%dllname%.dll -wd ..\result -dp ..\final_result\Microsoft.Kinect.dll

cd ..\result
call build
xcopy %dllname%.j4n.dll ..\final_result
xcopy %dllname%.j4n.jar ..\final_result

cd ..\lib
xcopy jni4net.j-0.8.8.0.jar ..\final_result
xcopy jni4net.n.w64.v40-0.8.8.0.dll ..\final_result
xcopy jni4net.n-0.8.8.0.dll ..\final_result

cd ..
echo Done!