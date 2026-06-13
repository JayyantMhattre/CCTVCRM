$modules = @(
    @{ Name = 'Lead'; Schema = 'cctv_lead'; HasDb = $true },
    @{ Name = 'Customer'; Schema = 'cctv_customer'; HasDb = $true },
    @{ Name = 'Amc'; Schema = 'cctv_amc'; HasDb = $true },
    @{ Name = 'Service'; Schema = 'cctv_service'; HasDb = $true },
    @{ Name = 'Ticket'; Schema = 'cctv_ticket'; HasDb = $true },
    @{ Name = 'Engineer'; Schema = 'cctv_engineer'; HasDb = $true },
    @{ Name = 'Invoice'; Schema = 'cctv_invoice'; HasDb = $true },
    @{ Name = 'Reporting'; Schema = ''; HasDb = $false }
)

$layers = @('Domain', 'Application', 'Infrastructure', 'Api')
$root = Join-Path $PSScriptRoot '..'

foreach ($m in $modules) {
    foreach ($layer in $layers) {
        $proj = "Ashraak.Cctv.$($m.Name).$layer"
        $dir = Join-Path $root "src/Modules/Cctv/$($m.Name)/$proj"
        New-Item -ItemType Directory -Force -Path $dir | Out-Null

        $lines = @(
            '<Project Sdk="Microsoft.NET.Sdk">',
            '  <PropertyGroup>',
            "    <RootNamespace>$proj</RootNamespace>",
            '  </PropertyGroup>'
        )

        if ($layer -eq 'Application') {
            $lines += '  <ItemGroup>'
            $lines += '    <PackageReference Include="MediatR" />'
            $lines += '    <PackageReference Include="FluentValidation" />'
            $lines += '    <PackageReference Include="FluentValidation.DependencyInjectionExtensions" />'
            $lines += '  </ItemGroup>'
            $lines += '  <ItemGroup>'
            $lines += "    <ProjectReference Include=`"..\Ashraak.Cctv.$($m.Name).Domain\Ashraak.Cctv.$($m.Name).Domain.csproj`" />"
            $lines += '    <ProjectReference Include="..\..\..\..\Shared\Ashraak.SharedKernel\Ashraak.SharedKernel.csproj" />'
            $lines += '    <ProjectReference Include="..\..\..\..\Shared\Ashraak.SharedKernel.Contracts\Ashraak.SharedKernel.Contracts.csproj" />'
            $lines += '    <ProjectReference Include="..\..\..\..\BuildingBlocks\Ashraak.BuildingBlocks.Application\Ashraak.BuildingBlocks.Application.csproj" />'
            $lines += '  </ItemGroup>'
        }
        elseif ($layer -eq 'Domain') {
            $lines += '  <ItemGroup>'
            $lines += '    <ProjectReference Include="..\..\..\..\Shared\Ashraak.SharedKernel\Ashraak.SharedKernel.csproj" />'
            $lines += '  </ItemGroup>'
        }
        elseif ($layer -eq 'Infrastructure') {
            if ($m.HasDb) {
                $lines += '  <ItemGroup>'
                $lines += '    <PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" />'
                $lines += '    <PackageReference Include="Microsoft.EntityFrameworkCore.Design" />'
                $lines += '    <PackageReference Include="MediatR" />'
                $lines += '  </ItemGroup>'
            }
            else {
                $lines += '  <ItemGroup>'
                $lines += '    <PackageReference Include="MediatR" />'
                $lines += '  </ItemGroup>'
            }
            $lines += '  <ItemGroup>'
            $lines += "    <ProjectReference Include=`"..\Ashraak.Cctv.$($m.Name).Application\Ashraak.Cctv.$($m.Name).Application.csproj`" />"
            $lines += '    <ProjectReference Include="..\..\..\..\Shared\Ashraak.SharedKernel.Contracts\Ashraak.SharedKernel.Contracts.csproj" />'
            $lines += '    <ProjectReference Include="..\..\..\..\BuildingBlocks\Ashraak.BuildingBlocks.Infrastructure\Ashraak.BuildingBlocks.Infrastructure.csproj" />'
            $lines += '    <ProjectReference Include="..\..\..\..\Infrastructure\Ashraak.Infrastructure.Shared\Ashraak.Infrastructure.Shared.csproj" />'
            $lines += '  </ItemGroup>'
        }
        elseif ($layer -eq 'Api') {
            $lines += '  <ItemGroup>'
            $lines += '    <FrameworkReference Include="Microsoft.AspNetCore.App" />'
            $lines += '  </ItemGroup>'
            $lines += '  <ItemGroup>'
            $lines += "    <ProjectReference Include=`"..\Ashraak.Cctv.$($m.Name).Application\Ashraak.Cctv.$($m.Name).Application.csproj`" />"
            $lines += '    <ProjectReference Include="..\..\..\..\Shared\Ashraak.SharedKernel.Contracts\Ashraak.SharedKernel.Contracts.csproj" />'
            $lines += '  </ItemGroup>'
        }

        $lines += '</Project>'
        Set-Content -Path (Join-Path $dir "$proj.csproj") -Value ($lines -join "`n") -Encoding UTF8
    }
}

Write-Host 'CCTV module projects generated.'
