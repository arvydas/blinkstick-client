#!/bin/sh

APP_NAME="BlinkStick Client.app"
BUILD=Release
BINARY=../BlinkStickClient/bin/$BUILD

mono $BINARY/BlinkStickClient.exe --build-config version.iss

VERSION_STRING=`sed -n 's/.*AppVersion \"\(.*\)\".*/\1/p' version.iss`
VERSION=`sed -n 's/.*AppFullVersion \"\([0-9]*\.[0-9]*\.[0-9]*\).*\".*/\1/p' version.iss`

OUTPUT=./setup/BlinkStickClient-$VERSION_STRING.pkg

RED='\x1b[31m'
GREEN='\x1b[32m'
NC='\x1b[0m'

echo "Building $RED$VERSION_STRING$NC ($VERSION)...\r\n"

mkdir -p ./setup
mkdir -p ./build

printf "Clean up if left over build exists..."
rm -rf "./build/$APP_NAME"
printf "$GREEN Done$NC\n"

printf "Prepare all directories..."
mkdir -p "./build/$APP_NAME"
mkdir -p "./build/$APP_NAME/Contents"
mkdir -p "./build/$APP_NAME/Contents/MacOS"
mkdir -p "./build/$APP_NAME/Contents/Resources"
printf "$GREEN Done$NC\n"

printf "Copy contents..."
cp -a ./osx/. "./build/$APP_NAME/Contents"
cp $BINARY/*.dll "./build/$APP_NAME/Contents/MacOS"
cp $BINARY/*.exe "./build/$APP_NAME/Contents/MacOS"
cp $BINARY/*.config "./build/$APP_NAME/Contents/MacOS"
cp $BINARY/*.ico "./build/$APP_NAME/Contents/MacOS"
cp $BINARY/*.png "./build/$APP_NAME/Contents/MacOS"
cp -a theme "./build/$APP_NAME/Contents/MacOS/Theme"
cp -a ../scripts "./build/$APP_NAME/Contents/MacOS/scripts"
printf "$GREEN Done$NC\n"

printf "Updating version info..."
sed -i '' -e "s/{VERSION}/$VERSION/g" "./build/$APP_NAME/Contents/Info.plist"
sed -i '' -e "s/{VERSION_STRING}/$VERSION_STRING/g" "./build/$APP_NAME/Contents/Info.plist"
printf "$GREEN Done$NC\n"

printf "Building PKG..."
pkgbuild --quiet --identifier com.agileinnovative.BlinkStickClient --root ./build --install-location /Applications $OUTPUT
printf "$GREEN Done$NC\n"

printf "Clean up..."
rm -rf "build/$APP_NAME"
printf "$GREEN Done$NC\n"

echo "\r\n\r\nBuild complete!"
echo "    $GREEN$OUTPUT$NC"
