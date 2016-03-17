#!/bin/sh

APP_NAME=BlinkStickClient.app

SOURCE=./osx

mkdir -p setup
mkdir -p build

rm -rf build/$APP_NAME


mkdir -p build/$APP_NAME
mkdir -p build/$APP_NAME/Contents
mkdir -p build/$APP_NAME/Contents/MacOS
mkdir -p build/$APP_NAME/Contents/Resources

cp -a $SOURCE/. build/$APP_NAME/Contents
cp ../BlinkStickClient/bin/Release/*.dll build/$APP_NAME/Contents/MacOS
cp ../BlinkStickClient/bin/Release/*.exe build/$APP_NAME/Contents/MacOS
cp ../BlinkStickClient/bin/Release/*.config build/$APP_NAME/Contents/MacOS
cp ../BlinkStickClient/bin/Release/*.ico build/$APP_NAME/Contents/MacOS
cp ../BlinkStickClient/bin/Release/*.png build/$APP_NAME/Contents/MacOS
cp -a theme build/$APP_NAME/Contents/MacOS/Theme
cp -a ../scripts build/$APP_NAME/Contents/MacOS/scripts
