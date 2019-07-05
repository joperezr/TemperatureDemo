dotnet publish -r linux-arm
pushd F:\scratch\TemperatureDemo\bin\Debug\netcoreapp3.0\linux-arm\publish\
scp * pi@joespi:/home/pi/TemperatureDemo/
popd