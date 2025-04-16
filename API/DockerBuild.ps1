docker build -t manhlong2712/gudfud-api .

if ($LASTEXITCODE -ne 0) {
    Write-Host "Build thất bại." -ForegroundColor Red
    # exit $LASTEXITCODE
} else {
    Write-Host "Build thành công." -ForegroundColor Green
}

pause