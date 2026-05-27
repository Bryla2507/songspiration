# Script to identify missing tabulature files
$apiUrl = "http://localhost:5036"
$uploadsDir = "songspiration/backend/SongSpiration.API/wwwroot/uploads"

# Get list of existing files
$existingFiles = Get-ChildItem $uploadsDir | Select-Object -ExpandProperty Name

# Try to fetch pins data from API
try {
    $response = Invoke-WebRequest -Uri "$apiUrl/api/Pins" -Method Get -ErrorAction Stop
    $pins = $response.Content | ConvertFrom-Json

    Write-Host "Analyzing pins data..."
    Write-Host "====================="

    $missingFiles = @()
    $foundFiles = @()

    foreach ($pin in $pins) {
        if ($pin.filePath -and $pin.filePath -match "/uploads/([^/]+)$") {
            $filename = $matches[1]
            if ($existingFiles -contains $filename) {
                $foundFiles += $filename
                Write-Host "✅ Found: $filename"
            } else {
                $missingFiles += $filename
                Write-Host "❌ Missing: $filename"
            }
        }
    }

    Write-Host "`nSummary:"
    Write-Host "========="
    Write-Host "Found files: $($foundFiles.Count)"
    Write-Host "Missing files: $($missingFiles.Count)"

    if ($missingFiles.Count -gt 0) {
        Write-Host "`nMissing files list:"
        $missingFiles | ForEach-Object { Write-Host "- $_" }
    }

} catch {
    Write-Host "Error connecting to API: $_"
    Write-Host "Please make sure the backend is running on $apiUrl"
}