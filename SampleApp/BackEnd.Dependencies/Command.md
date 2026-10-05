Lessons from

https://app.pluralsight.com/ilx/video-courses/9908dc74-863d-4fee-b3b4-6d796ecbbf3a/71bcd97f-50c3-4f58-ac90-39966ec2be4b/ad65717c-d2c7-4712-aad3-554736967f7f

https://app.pluralsight.com/ilx/video-courses/fcae0ff9-67ad-4398-a04f-20894693e177/475220f5-9616-4735-858a-d05cd870f7ee/be46a385-111a-4bd0-88bc-aac786b9e657

dotnet list package

Restaurer a réussi avec 8 avertissement(s) en 7,9s
    /home/Luke/Documents/Workspace/dotnet/GitHub/asp.net_best_practices/SampleApp/BackEnd.Dependencies/BackEnd.Dependencies.csproj : warning NU1903: Le package 'Azure.Identity' 1.7.0 présente une vulnérabilité de gravité élevé(e) connue, https://github.com/advisories/GHSA-5mfx-4wcx-rv27.
    /home/Luke/Documents/Workspace/dotnet/GitHub/asp.net_best_practices/SampleApp/BackEnd.Dependencies/BackEnd.Dependencies.csproj : warning NU1902: Le package 'Azure.Identity' 1.7.0 présente une vulnérabilité de gravité moyenne connue, https://github.com/advisories/GHSA-m5vv-6r4h-3vj9.
    /home/Luke/Documents/Workspace/dotnet/GitHub/asp.net_best_practices/SampleApp/BackEnd.Dependencies/BackEnd.Dependencies.csproj : warning NU1902: Le package 'Azure.Identity' 1.7.0 présente une vulnérabilité de gravité moyenne connue, https://github.com/advisories/GHSA-wvxc-855f-jvrv.
    /home/Luke/Documents/Workspace/dotnet/GitHub/asp.net_best_practices/SampleApp/BackEnd.Dependencies/BackEnd.Dependencies.csproj : warning NU1903: Le package 'Microsoft.Data.SqlClient' 5.1.1 présente une vulnérabilité de gravité élevé(e) connue, https://github.com/advisories/GHSA-98g6-xh36-x2p7.
    /home/Luke/Documents/Workspace/dotnet/GitHub/asp.net_best_practices/SampleApp/BackEnd.Dependencies/BackEnd.Dependencies.csproj : warning NU1903: Le package 'Microsoft.Extensions.Caching.Memory' 8.0.0 présente une vulnérabilité de gravité élevé(e) connue, https://github.com/advisories/GHSA-qj66-m88j-hmgj.
    /home/Luke/Documents/Workspace/dotnet/GitHub/asp.net_best_practices/SampleApp/BackEnd.Dependencies/BackEnd.Dependencies.csproj : warning NU1902: Le package 'Microsoft.IdentityModel.JsonWebTokens' 6.24.0 présente une vulnérabilité de gravité moyenne connue, https://github.com/advisories/GHSA-59j7-ghrg-fj52.
    /home/Luke/Documents/Workspace/dotnet/GitHub/asp.net_best_practices/SampleApp/BackEnd.Dependencies/BackEnd.Dependencies.csproj : warning NU1903: Le package 'Newtonsoft.Json' 9.0.1 présente une vulnérabilité degravité élevé(e) connue, https://github.com/advisories/GHSA-5crp-9r3c-p9vr.
    /home/Luke/Documents/Workspace/dotnet/GitHub/asp.net_best_practices/SampleApp/BackEnd.Dependencies/BackEnd.Dependencies.csproj : warning NU1902: Le package 'System.IdentityModel.Tokens.Jwt' 6.24.0 présente unevulnérabilité de gravité moyenne connue, https://github.com/advisories/GHSA-59j7-ghrg-fj52.

Générer a réussi avec 8 avertissement(s) dans 8,9s
Le projet 'BackEnd.Dependencies' a les références de package suivantes
   [net10.0]: 
   Package de niveau supérieur                    Demandé   Résolu 
   > Azure.Storage.Blobs                          12.19.1   12.19.1
   > Microsoft.EntityFrameworkCore.SqlServer      8.0.1     8.0.1  
   > Newtonsoft.Json                              9.0.1     9.0.1  
   > Polly                                        8.2.0     8.2.0  
   > Serilog.AspNetCore                           8.0.0     8.0.0  



dotnet list package --include-transitive
dotnet list package --include-transitive --format json 
dotnet list package --vulnerable


Le projet 'BackEnd.Dependencies' comporte les packages vulnérables suivants
   [net10.0]: 
   Package de niveau supérieur      Demandé   Résolu   Gravité   URL d'avertissement       
   > Newtonsoft.Json                9.0.1     9.0.1    High      https://github.com/advisories/GHSA-5crp-9r3c-p9vr

dotnet tool install --gloabl CycloneDX
