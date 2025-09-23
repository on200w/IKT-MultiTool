<h1 align="center">🛠️ IKT-MultiTool v2.0</h1>

<p align="center">
  <em>Alt-i-ett Windows-verktøy for IT-support, feilsøking, vedlikehold og driver-administrasjon</em>
</p>

<hr>

<h2>🆕 Nye funksjoner i v2.0</h2>
<ul>
  <li>💾 <strong>Nytt Driver-panel</strong> – Dynamisk administrasjon av PC-modeller og drivere</li>
  <li>➕ <strong>Legg til PC-modeller</strong> – Opprett nye PC-modeller direkte i programmet</li>
  <li>📦 <strong>Driver-installasjon</strong> – Enkelt installere drivere med ett klikk</li>
  <li>📂 <strong>Automatisk mappestruktur</strong> – Mapper opprettes automatisk for nye PC-modeller</li>
  <li>🗑️ <strong>Fjernet ubrukte paneler</strong> – Brukeradministrasjon og Diverse verktøy er fjernet</li>
  <li>🎯 <strong>Strømlinjeformet grensesnitt</strong> – Kun aktuelle verktøy vises</li>
  <li>🔧 <strong>Generelle driver-verktøy</strong> – Fungerer på tvers av alle PC-modeller</li>
</ul>

<hr>

<h2>✨ Funksjoner</h2>
<ul>
  <li>🖥️ <strong>System</strong> – Maskinvare, drivere, diskplass, prosesser, systeminfo</li>
  <li>🌐 <strong>Nettverk</strong> – IP-konfigurasjon, MAC-adresser, Wi-Fi-profiler, hastighetstest, ping, traceroute</li>
  <li>� <strong>Drivere</strong> – Administrer drivere for forskjellige PC-modeller, installer automatisk</li>
  <li>🛠️ <strong>Feilsøking og vedlikehold</strong> – SFC, DISM, Windows Update, diskopprydding, papirkurv</li>
  <li>�️ <strong>Office-cache</strong> – Rydd opp i Teams, OneNote og Office-cache, fiks error 48v35</li>
  <li>🎨 <strong>Moderne UI</strong> – Mørkt tema, store knapper, konsistent design med lilla fargepalett</li>
</ul>

<hr>

<h2>💾 Driver-administrasjon</h2>
<h3>Støttede funksjoner:</h3>
<ul>
  <li><strong>Dynamisk PC-modell støtte</strong> – Legg til nye PC-modeller uten koding</li>
  <li><strong>Automatisk driver-telling</strong> – Viser antall tilgjengelige drivere per modell</li>
  <li><strong>Støttede filtyper</strong> – .exe, .msi, .zip driver-filer</li>
  <li><strong>Administrator-installasjon</strong> – Automatisk forhøyede rettigheter for driver-installasjon</li>
  <li><strong>Mappeadministrasjon</strong> – Åpne driver-mapper direkte fra programmet</li>
</ul>

<h3>Slik legger du til drivere:</h3>
<ol>
  <li>Åpne "💾 Drivere" fra hovedmenyen</li>
  <li>Klikk "➕ Legg til ny PC-modell" hvis din modell ikke finnes</li>
  <li>Velg PC-modellen din</li>
  <li>Klikk "📂 Åpne drivermappe for denne PC-en"</li>
  <li>Kopier driver-filer (.exe, .msi, .zip) til mappen</li>
  <li>Gå tilbake og klikk på en driver for å installere</li>
</ol>

<hr>

<h2>📦 Krav</h2>
<ul>
  <li>Windows 10/11</li>
  <li>.NET 8.0</li>
  <li>Visual Studio eller VS Code</li>
</ul>

<hr>

<h2>🚀 Bruk</h2>
<ol>
  <li><strong>Klon repoet</strong> eller last ned filen:
    <pre><code>git clone https://github.com/on200w/IKT-MultiTool.git
cd IKT-MultiTool</code></pre>
  </li>
  <li><strong>Åpne i Visual Studio eller VS Code</strong></li>
  <li><strong>Kjør programmet</strong>:
    <pre><code>dotnet build
dotnet run</code></pre>
  </li>
<h2>🚀 Bruk</h2>
<ol>
  <li><strong>Klon repoet</strong> eller last ned filen:
    <pre><code>git clone https://github.com/on200w/IKT-MultiTool.git
cd IKT-MultiTool</code></pre>
  </li>
  <li><strong>Åpne i Visual Studio eller VS Code</strong></li>
  <li><strong>Kjør programmet</strong>:
    <pre><code>dotnet build
dotnet run</code></pre>
    <em>Merk: Programmet kan kreve administrator-rettigheter for enkelte funksjoner</em>
  </li>
  <li><strong>Bruk</strong>:
    <ul>
      <li>Velg ønsket panel fra hovedmenyen (System, Nettverk, Drivere, Feilsøking, Cache)</li>
      <li>For drivere: Legg til PC-modeller og administrer driver-filer enkelt</li>
      <li>Trykk på relevante knapper for feilsøking, vedlikehold eller info</li>
      <li>Følg instruksjonene for Office error 48v35 eller andre problemer</li>
    </ul>
  </li>
</ol>

<hr>

<h2>� Prosjektstruktur</h2>
<pre><code>IKTMultiTool/
├── drivers/                    # Driver-filer organisert per PC-modell
│   ├── Lenovo_Thinkpad_E14_Gen6/
│   └── README.md
├── icon/                       # Programikoner
├── Form1.cs                    # Hovedform og navigasjon
├── DriverPanel.cs              # Driver-administrasjon
├── NetworkPanel.cs             # Nettverksverktøy
├── SystemPanel.cs              # Systemverktøy
├── MaintenancePanel.cs         # Vedlikeholdsverktøy
├── OfficeCachePanel.cs         # Office cache-verktøy
├── Logger.cs                   # Loggføring
└── README.md                   # Denne filen
</code></pre>

<hr>

<h2>�🖼 Hvordan programmet ser ut</h2>
<p align="center">
  <img src="https://i.postimg.cc/QM27DM27/image.png" alt="IKT MultiTool hovedmeny" width="500"><br>
  <em>Hovedmeny v2.0: Strømlinjeformet med System, Nettverk, Drivere, Feilsøking og Cache</em>
  <br><br>
  <img src="https://i.postimg.cc/ZnCzLTqf/image.png" alt="Driver panel" width="500"><br>
  <em>Driver-panel: Administrer PC-modeller og installer drivere enkelt</em>
    <br><br>
  <img src="https://i.postimg.cc/XvYWvcbS/image.png" alt="Office 48v35 Error code" width="500"><br>
  <em>Office-panel: Fikks 48v35 error koden raskt</em>
</p>

<hr>

<h2>🔧 Teknisk informasjon</h2>
<ul>
  <li><strong>Framework:</strong> .NET 8.0 Windows Forms</li>
  <li><strong>Språk:</strong> C#</li>
  <li><strong>UI:</strong> Windows Forms med mørkt tema</li>
  <li><strong>Avhengigheter:</strong> System.Management, Newtonsoft.Json</li>
  <li><strong>Kompatibilitet:</strong> Windows 10/11</li>
</ul>

<hr>

<h2>📝 Endringslogg</h2>
<h3>v2.0 (2025)</h3>
<ul>
  <li>➕ Lagt til komplett driver-administrasjonssystem</li>
  <li>➕ Dynamisk PC-modell støtte</li>
  <li>➕ Automatisk mappestruktur for drivere</li>
  <li>🗑️ Fjernet ubrukte paneler (Brukeradministrasjon, Diverse verktøy)</li>
  <li>🎨 Forbedret UI konsistens</li>
</ul>

<h3>v1.0 (2024)</h3>
<ul>
  <li>🎉 Første versjon med grunnleggende IT-verktøy</li>
  <li>🎨 Mørkt tema og lilla fargepalett</li>
  <li>🛠️ System, nettverk, og vedlikeholdsverktøy</li>
  <li>🗑️ Office cache-rydding</li>
</ul>
