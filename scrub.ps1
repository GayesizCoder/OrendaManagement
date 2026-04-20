$path = "Orenda.Web/appsettings.json"
if (Test-Path $path) {
    $content = Get-Content $path -Raw
    $content = $content -replace 'Server=.*?;', 'Server=YOUR_SERVER;'
    $content = $content -replace 'Database=.*?;', 'Database=YOUR_DB;'
    $content = $content -replace 'User Id=.*?;', 'User Id=YOUR_USER;'
    $content = $content -replace 'Password=.*?;', 'Password=YOUR_PASSWORD;'
    Set-Content $path $content -Encoding UTF8
}
