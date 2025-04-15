dotnet publish -c Release -o "Publish"

if ($LASTEXITCODE -ne 0) {
    Write-Host "Build thất bại." -ForegroundColor Red
    # exit $LASTEXITCODE
} else {
    Write-Host "Build thành công." -ForegroundColor Green

    docker build -t manhlong2712/gudfud-api .
}

pause