<h1 align="center">🛠️ IKT-MultiTool v2.1</h1>

<p align="center">
  <em>Alt-i-ett Windows-verktøy for IT-support, feilsøking, vedlikehold og driver-administrasjon</em>
</p>

<hr>

<h2>🆕 Nytt i v2.1 (design)</h2>
<ul>
  <li>📐 65/35-layout – knappesiden ~35%, kommandovisning ~65%</li>
  <li>⬅️ Full bredde mot venstre – knapper og overskrifter fyller hele venstre panel</li>
  <li>🅰️ Venstrejustert tekst, jevn knapphøyde (60px), ryddige marginer</li>
  <li>😀 Emoji-støtte (Segoe UI Emoji) over hele appen; Office-panelet viser emoji korrekt</li>
  <li>💜 Lilla tema konsistent på knapper/utdata</li>
  <li>🧭 Driver-panelet: seksjonsoverskrifter/labels har større bredde og følger størrelsen dynamisk</li>
</ul>

<hr>

<h2>✨ Funksjoner</h2>
  <li>🖥️ <strong>System</strong> – Maskinvare, drivere, diskplass, prosesser, systeminfo</li>
  <li>🌐 <strong>Nettverk</strong> – IP-konfigurasjon, MAC-adresser, Wi-Fi-profiler, hastighetstest, ping, traceroute</li>
  <li>� <strong>Drivere</strong> – Administrer drivere for forskjellige PC-modeller, installer automatisk</li>
  <li>🛠️ <strong>Feilsøking og vedlikehold</strong> – SFC, DISM, Windows Update, diskopprydding, papirkurv</li>
  <li>�️ <strong>Office-cache</strong> – Rydd opp i Teams, OneNote og Office-cache, fiks error 48v35</li>
  <li>🎨 <strong>Moderne UI</strong> – Mørkt tema, store knapper, konsistent design med lilla fargepalett</li>
</ul>


<hr>
  <li>💾 <strong>Drivere</strong> – Administrer drivere for forskjellige PC-modeller, installer automatisk</li>
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
  <li>.NET 8.0 (kun hvis du kjører framework-avhengig build)</li>
  <li>Velg PC-modellen din</li>
  <li>Klikk "📂 Åpne drivermappe for denne PC-en"</li>
  <li>Kopier driver-filer (.exe, .msi, .zip) til mappen</li>
<blockquote>
  <strong>Distribusjon:</strong> v2.1 bygger som standard en <em>single-file</em>, <em>self-contained</em> EXE for <code>win-x64</code> og krever administrator (UAC). Ingen .NET-installasjon trengs på målmaskinen.
</blockquote>

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
  <li><strong>Kjør programmet (dev)</strong>:
    <pre><code class="language-powershell">dotnet build
dotnet run</code></pre>
    <em>Merk: Appen ber om administrator ved oppstart (manifest).</em>
  </li>
  <li><strong>Publiser single-file (release)</strong>:
    <pre><code class="language-powershell">dotnet publish -c Release -r win-x64 -p:PublishSingleFile=true -p:SelfContained=true</code></pre>
    <ul>
      <li>Output: <code>IKTMultiTool\bin\Release\net8.0-windows\win-x64\publish\IKTMultiTool.exe</code></li>
      <li>EXE er selvstendig (inkluderer runtime) og vil vise UAC-spørsmål ved oppstart.</li>
    </ul>
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
  </li>
<h2>� Prosjektstruktur</h2>
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
<h2>🖼 Skjermbilder</h2>
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
<h3>v2.1 (2025)</h3>
<ul>
  <li>📐 65/35-layout mellom knappeside og visning</li>
  <li>⬅️ Knapper og overskrifter fyller venstre panel i full bredde</li>
  <li>😀 Emoji-støtte overalt; Office-panel emoji fikset</li>
  <li>💜 Konsistent lilla tema for knapper/utdata</li>
  <li>🧭 Driver-panel: bredere seksjonslabels og dynamisk bredde</li>
  <li>🧰 Små UI-forbedringer: marginer, venstrejustert tekst, 60px knapphøyde</li>
  <li>📦 Publisering: standard single-file, self-contained EXE (win-x64) med admin-krav</li>
  <li>🔒 Administrator via manifest (UAC) ved oppstart</li>
</ul>
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
