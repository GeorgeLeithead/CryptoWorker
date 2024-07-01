$StartedDate = Get-Date('2024-06-30')
$CurrentDate = Get-Date
$ts = New-TimeSpan -Start $StartedDate $CurrentDate
$NumDaysToGet = $ts.Days
$StartNumb = 1 # Today
$StartNumb = $StartNumb+1

# Run against the markets*.json file in reverse order (i.e. 1.5, 1.4, 1.3, 1.2, etc)

$Logs = git log -$NumDaysToGet --pretty=format:"%h" .\Markets1.4.json
for($i = 1; $i -le $NumDaysToGet; $i++)
{
    $Commit = $Logs[$i-1]
    if ($null -ne $Commit)
    {
        [datetime]$CommitDate = git show --no-patch --format="%ci" $Commit
        [string]$FileName = (get-date $CommitDate -format yyyy-MM-dd) + ".json"
        if (Test-Path -path .\$FileName -PathType Leaf)
        {
            write-host "already exists"
        }
        else {
            write-Host "Commit is $($Commit) for date $($FileName)"
            # if you change markets.json above, make SURE you change this to match!!!
            git show $Commit`:Markets1.4.json > $FileName
        }
    }
}
