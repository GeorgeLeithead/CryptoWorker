$Uname = ""
$Pwd = ""
$DataSource = ""
$Symbols = invoke-sqlcmd -ConnectionString "User ID=$($Uname);Password=$($Pwd);Initial Catalog=Crypto;Data Source=$($DataSource),1433" -Query "SELECT * FROM Symbol"

Get-ChildItem -Path .\History -File -Filter *.json | Sort-Object Name | ForEach-Object {
    #Write-Host "*** Getting local $($HistoryFile.Name) ***"
    Write-Output "INSERT INTO Ranking ([SymbolID], [Rank], [RankDate]) VALUES"
    $MarketHistroy = Get-Content -path $_.FullName | ConvertFrom-Json
    foreach($Market in $MarketHistroy.Markets)
    {
        if ($Market.SymbolString -in $Symbols.SymbolName)
        {
            $Sym = $Symbols | Where-Object { $_.SymbolName -eq $Market.SymbolString }
#            write-Host "$($Market.SymbolString) was rank $($Market.Rank) on $($HistoryFile.Name.TrimEnd('.json'))"
            #Write-Output "new Ranking{SymbolID = $($Sym.ID), Rank = $($Market.Rank), RankDate = `"$($_.Name.TrimEnd('.json'))`",},"
            Write-Output "($($Sym.ID), $($Market.Rank), '$($_.Name.TrimEnd('.json'))'),"
        }
        else
        {
#            Write-Host "('$($Market.SymbolString)', '($($Market.SymbolString))', $($Market.DecimalPlaces), 0),"
        }

    }

    Write-Output "GO"
} | Out-File .\insert.txt

