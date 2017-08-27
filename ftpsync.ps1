param(
    [string]$uri,
    [string]$username,
    [SecureString]$password,
    [string]$Dir = "/",
    [string]$DestinationDirectory = "",
    [boolean]$dryRun = $true,
    [boolean]$onlyDownloadNew = $true,
    [boolean]$Recursive = $true,
    [boolean]$KeepFolderStructure = $false
)

$start = [System.DateTime]::Now
$previouslyDownloadedFile = ".prev.txt"
$logFile = "log.txt"
# Remove SSL Certification validation as we trust our target.
[Net.ServicePointManager]::ServerCertificateValidationCallback = {$true}

if (!$Dir) {
    $Dir = ""
}

if (!$DestinationDirectory) {
    $DestinationDirectory = $PSScriptRoot
}

function Ftp([string]$FtpEndpoint, [boolean]$Download){
    $ftp = [System.Net.FtpWebRequest]::create($FtpEndpoint)
    $ftp.Credentials = New-Object System.Net.NetworkCredential($username,$password)
    $ftp.UseBinary = $true
    $ftp.EnableSsl = $true
    $ftp.KeepAlive = $true
    $ftp.Timeout = -1
    if ($Download) { 
        $ftp.Method = [System.Net.WebRequestMethods+Ftp]::DownloadFile 
    } else { 
        $ftp.Method = [System.Net.WebRequestMethods+Ftp]::ListDirectoryDetails 
    }
    $ftp
}

function GetFiles([string]$Path){
    # Create a FTPWebRequest object to handle the connection to the ftp server
    $absoluteUri = $uri + $Path
    $ftp = Ftp -FtpEndpoint $absoluteUri -Download $false

    try {
        $response = $ftp.GetResponse()
    }
    catch {
        if ($response) { $response.Close() }
        Log -Message $_; Write-Warning $_; break;
    }

    $reader = New-Object IO.StreamReader $response.GetResponseStream()
    $directories = @()
    $files = New-Object System.Collections.ArrayList

    [string]$line = $reader.ReadLine()
    while($line) {
        $null, [string]$directory, [string]$lineFlag, [string]$lineLink, [string]$lineUsername, [string]$lineGroup, [string]$lineSize, [string]$lineDate, [string]$name = `
        [regex]::split($Line,'^([d-])([rwxt-]{9})\s+(\d{1,})\s+([.@A-Za-z0-9-]+)\s+([A-Za-z0-9-]+)\s+(\d{1,})\s+(\w+\s+\d{1,2}\s+\d{1,2}:?\d{2})\s+(.+?)\s?$',"SingleLine,IgnoreCase,IgnorePatternWhitespace")
        
        $fullPath = $Path + "/" + $name.Trim()
        if ($directory -eq "d" -or $directory -eq "DIR") {
            $directories += $fullPath
        } else {
            #$files += $fullPath
            $result = $files.Add($fullPath)
        }

        $line = $reader.ReadLine()
    }

    #Close
    $result = $reader.Close()
    $result = $response.Close()

    if ($Recursive) {
        foreach($subDirectory in $directories) {
            $moreFiles = GetFiles -Path $subDirectory
            #$files += $moreFiles
            $files.AddRange($moreFiles)
        }
    }

    return $files
}

function DownloadFile([string]$File) {
    $fullPath = $uri + "/" + $File
    $fileName = ($File -split "/")[-1]
    $fileStart = [System.DateTime]::Now

    $root = $DestinationDirectory
    if ($KeepFolderStructure) {
        $folders = ($File -split "/" | Select-Object -Skip 1)
        $root = Join-Path -Path $root -ChildPath $folders
    }
    $destinationPath = $fileName
    if ($DestinationDirectory) {
        $destinationPath = Join-Path $root -ChildPath $fileName
    }
    if (Test-Path $destinationPath) { 
        # Remove-Item $destinationPath -Force
        $destinationPath = $destinationPath + [System.Guid]::NewGuid().ToString()
    }

    Log -Message "Starting download $File to $destinationPath."
    $ftp = Ftp -FtpEndpoint $fullPath -Download $true

    try { $response = $ftp.GetResponse() }
    catch {
        if ($response) { $response.Close() }
        Log -Message $_; Write-Warning $_; break;
    }
    $stream = $response.GetResponseStream()
    $reader = New-Object IO.StreamReader $stream
    
    try {
        # $reader.ReadToEnd() | Out-File $destinationPath # Doesnt work file size wrong

        $offset = 0
        $bufferSize = 1024
        $fileInfo = New-Object IO.FileStream ($destinationPath, [IO.FileMode]::OpenOrCreate)
        [byte[]]$buffer = New-Object byte[] $bufferSize
        do {  
            $length = $stream.Read($buffer, 0, $bufferSize)  
            $fileInfo.Write($buffer, 0, $length)  
            $offset = $offset + $length
        } while ($length -ne 0)
        $fileInfo.Close()

        $reader.Close()
        $response.Close()

        $size = (Get-Item $destinationPath).length
        $fileEnd = [System.DateTime]::Now
        $fileDownloadTime = [System.Math]::Round(($fileEnd - $fileStart).TotalSeconds, 2)
        $kbps = [System.Math]::Round(($size / 1024) / (($fileEnd - $fileStart).TotalSeconds), 2)
        
        Log -Message "Downloaded $destinationPath in $fileDownloadTime seconds for $size($offset) bytes. $kbps kbps."

        if ($onlyDownloadNew) {
            $File | Out-File $previouslyDownloadedFile -Append
        }
    }
    catch {
        Log -Message $_; Write-Warning $_; break;
    }
}

function Log(){ Param([string]$Message)
    $outText = "$([System.DateTime]::Now.ToString("yyyy-MM-dd H:mm:ss")): $Message"
    $outText | Out-File $logFile -Append
    Write-Host $outText
}

function Main() {
    # Retrieve a list of all the files to download.
    [array]$allFiles = GetFiles $Dir
    $toDownload = New-Object System.Collections.ArrayList
    $toDownload.AddRange($allFiles)

    # If only downloading new files, remove all the previous ones from the list.
    if ($onlyDownloadNew) {
        $previous = Get-Content $previouslyDownloadedFile
        foreach ($previousFile in $previous) {
            $toDownload.Remove($previousFile)
        }
    }

    # Download each file.
    foreach ($downloadFile in $toDownload) {
        DownloadFile -File $downloadFile
    }

    Log -Message "Finished total run time $([System.Math]::Round(([System.DateTime]::Now - $start).TotalSeconds, 2)) seconds. Checked $($allFiles.Length) files, $($toDownload.Count) downloaded."
}

Main