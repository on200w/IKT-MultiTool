using System.Management; // For WMI-spørring
using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace IKTMultiTool
{
    public partial class SystemPanel : UserControl
    {
        private TextBox outputBox;
        private SplitContainer split;
        public event Action? OnBack;
        public SystemPanel()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.FromArgb(30, 30, 30);
            split = new SplitContainer {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical,
                BackColor = Color.FromArgb(30, 30, 30),
                BorderStyle = BorderStyle.FixedSingle,
                IsSplitterFixed = true
            };
            this.Resize += (s, e) => split.SplitterDistance = (int)(this.Width * 0.35);
            var layout = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoScroll = true, FlowDirection = FlowDirection.TopDown, WrapContents = false, BackColor = Color.FromArgb(30, 30, 30) };
            var btnBack = new Button { Text = "⬅ Tilbake", AutoSize = true, BackColor = Color.FromArgb(45,45,45), ForeColor = Color.White, Font = new Font("Segoe UI Emoji", 10, FontStyle.Bold), FlatStyle = FlatStyle.Flat, Margin = new Padding(5) };
            btnBack.Click += (s, e) => OnBack?.Invoke();
            layout.Controls.Add(btnBack);
            layout.Controls.Add(new Label { Height = 20, Width = 1 });
            string[] btnTexts = {
                "🖥️ Systeminformasjon", "📋 Liste over kjørende prosesser", "💽 Diskplass", "🏷️ Maskinnavn og brukernavn",
                "⏱️ Oppetid", "🔧 Maskinvareinfo (CPU/RAM)", "📂 Liste drivere", "🌡️ Temperatur (WMI)"
            };
            int maxBtnWidth = btnBack.Width;
            for (int i = 0; i < btnTexts.Length; i++)
            {
                var btn = new Button
                {
                    Text = btnTexts[i],
                    AutoSize = true,
                    BackColor = Color.FromArgb(45, 45, 45),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI Emoji", 10, FontStyle.Bold),
                    FlatStyle = FlatStyle.Flat,
                    Margin = new Padding(5)
                };
                layout.Controls.Add(btn);
                if (btn.Width > maxBtnWidth) maxBtnWidth = btn.Width;
                switch (i)
                {
                    case 0:
                        btn.Click += (s, e) => RunCmd("systeminfo");
                        break;
                    case 1:
                        btn.Click += (s, e) => RunCmd("tasklist");
                        break;
                    case 2:
                        btn.Click += (s, e) => ShowDiskSpace();
                        break;
                    case 3:
                        btn.Click += (s, e) => RunCmd("echo Maskinnavn: %COMPUTERNAME% & echo Bruker: %USERNAME%");
                        break;
                    case 4:
                        btn.Click += (s, e) => RunCmd("net statistics workstation");
                        break;
                    case 5:
                        btn.Click += (s, e) => RunCmd("powershell -NoProfile -Command \"Get-CimInstance Win32_Processor | Select-Object Name,NumberOfCores,NumberOfLogicalProcessors,MaxClockSpeed | Format-Table; Get-CimInstance Win32_PhysicalMemory | Select-Object Manufacturer,PartNumber,Capacity,Speed | Format-Table\"");
                        break;
                    case 6:
                        btn.Click += (s, e) => ListDrivers();
                        break;
                    case 7:
                        btn.Click += (s, e) => { if (outputBox != null) outputBox.Text = GetCpuTemperature(); };
                        break;
                }
            }
            // Dynamisk juster SplitterDistance hvis knappene er bredere enn menyområdet
            int minMenuWidth = maxBtnWidth + 40; // 40px margin
            if (split.SplitterDistance < minMenuWidth)
                split.SplitterDistance = minMenuWidth;
            outputBox = new TextBox { Multiline = true, Dock = DockStyle.Fill, ReadOnly = true, BackColor = Color.Black, ForeColor = Color.Lime, Font = new Font("Consolas", 10), ScrollBars = ScrollBars.Vertical, BorderStyle = BorderStyle.FixedSingle };
            split.Panel1.Controls.Add(layout);
            split.Panel2.Controls.Add(outputBox);
            this.Controls.Add(split);
            Color lilla = Color.FromArgb(120, 60, 200);
            foreach (var btn in layout.Controls.OfType<Button>())
            {
                btn.BackColor = lilla;
                btn.ForeColor = Color.White;
            }
            outputBox.ForeColor = lilla;
        }
        public string GetCpuTemperature()
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(@"root\WMI", "SELECT * FROM MSAcpi_ThermalZoneTemperature");
                foreach (ManagementObject obj in searcher.Get())
                {
                    double temp = Convert.ToDouble(obj["CurrentTemperature"].ToString());
                    // Temperatur er i Kelvin * 10, konverter til Celsius:
                    temp = (temp / 10) - 273.15;
                    return $"CPU Temperatur: {temp:F1} °C";
                }
                return "Ingen temperaturdata funnet.";
            }
            catch (Exception ex)
            {
                return $"Feil: {ex.Message}";
            }
        }
        private async void RunCmd(string cmd)
        {
            try
            {
                outputBox.Text = "Kjører kommando...\r\n";
                var psi = new ProcessStartInfo("cmd.exe", $"/c {cmd}")
                {
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                var proc = Process.Start(psi);
                if (proc != null)
                {
                    while (!proc.StandardOutput.EndOfStream)
                    {
                        string? line = await proc.StandardOutput.ReadLineAsync();
                        if (line != null) outputBox.AppendText(line + "\r\n");
                    }
                    string error = await proc.StandardError.ReadToEndAsync();
                    if (!string.IsNullOrWhiteSpace(error))
                        outputBox.AppendText(error);
                    proc.WaitForExit();
                }
                else
                {
                    outputBox.Text = "Kunne ikke starte prosess.";
                }
            }
            catch (Exception ex)
            {
                outputBox.Text = "Feil: " + ex.Message;
            }
        }
        private async void ListDrivers()
        {
            try
            {
                outputBox.Text = "Henter driveroversikt...\r\n";
                var psi = new ProcessStartInfo("cmd.exe", "/c driverquery /v /fo table")
                {
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                var proc = Process.Start(psi);
                if (proc != null)
                {
                    string output = await proc.StandardOutput.ReadToEndAsync();
                    proc.WaitForExit();
                    var lines = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    if (lines.Length > 2)
                    {
                        var header = lines[0];
                        int nameStart = header.IndexOf("Module Name");
                        int typeStart = header.IndexOf("Driver Type");
                        int stateStart = header.IndexOf("State");
                        int pathStart = header.IndexOf("Path");
                        int dateStart = header.IndexOf("Date");
                        outputBox.Text = "Navn           | Type         | Status   | Filsti                      | Dato\r\n";
                        outputBox.AppendText(new string('-', 100) + "\r\n");
                        for (int i = 2; i < lines.Length; i++) // hopp over header og strek
                        {
                            string line = lines[i];
                            // Vis kun linjer som har tekst i Module Name
                            if (string.IsNullOrWhiteSpace(line) || (nameStart >= 0 && string.IsNullOrWhiteSpace(line.Substring(nameStart, Math.Min(14, line.Length - nameStart)).Trim()))) continue;
                            string navn = nameStart >= 0 && line.Length > nameStart ? line.Substring(nameStart, Math.Min(14, line.Length - nameStart)).Trim() : "";
                            string type = typeStart >= 0 && line.Length > typeStart ? line.Substring(typeStart, Math.Min(12, line.Length - typeStart)).Trim() : "";
                            string status = stateStart >= 0 && line.Length > stateStart ? line.Substring(stateStart, Math.Min(8, line.Length - stateStart)).Trim() : "";
                            string filsti = pathStart >= 0 && line.Length > pathStart ? line.Substring(pathStart, Math.Min(40, line.Length - pathStart)).Trim() : "";
                            string dato = dateStart >= 0 && line.Length > dateStart ? line.Substring(dateStart).Trim() : "";
                            outputBox.AppendText($"{navn,-14} | {type,-12} | {status,-8} | {filsti,-40} | {dato}\r\n");
                        }
                    }
                    else
                    {
                        outputBox.Text = "Fant ingen drivere.";
                    }
                }
                else
                {
                    outputBox.Text = "Kunne ikke starte prosess.";
                }
            }
            catch (Exception ex)
            {
                outputBox.Text = "Feil: " + ex.Message;
            }
        }
        private async void ShowDiskSpace()
        {
            outputBox.Text = "Kjører kommando...\r\n";
            try
            {
                var psi = new ProcessStartInfo("powershell", "-NoProfile -Command \"Get-PSDrive -PSProvider 'FileSystem' | Select Name,Used,Free | ConvertTo-Json\"")
                {
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                var proc = Process.Start(psi);
                if (proc != null)
                {
                    string json = await proc.StandardOutput.ReadToEndAsync();
                    proc.WaitForExit();
                    var drives = JsonConvert.DeserializeObject<dynamic>(json);
                    outputBox.Text = "";
                    if (drives != null)
                    {
                        foreach (var d in drives)
                        {
                            string? name = d?.Name;
                            double used = d?.Used != null ? (double)d.Used / (1024 * 1024 * 1024) : 0;
                            double free = d?.Free != null ? (double)d.Free / (1024 * 1024 * 1024) : 0;
                            double total = used + free;
                            int barLength = 30;
                            int usedBar = total > 0 ? (int)Math.Round(barLength * used / total) : 0;
                            int freeBar = barLength - usedBar;
                            string bar = new string('█', usedBar) + new string('░', freeBar);
                            outputBox.AppendText($"Stasjon {name}:\r\nBrukt: {used:F1} GB / {total:F1} GB\r\n[{bar}]\r\nLedig: {free:F1} GB\r\n\r\n");
                        }
                    }
                    else
                    {
                        outputBox.Text += "Feil: Ingen diskdata tilgjengelig.";
                    }
                }
                else
                {
                    outputBox.Text += "Feil: Kunne ikke starte PowerShell-prosess.";
                }
            }
            catch (Exception ex)
            {
                outputBox.Text += $"Feil: {ex.Message}";
            }
        }
    }
}
