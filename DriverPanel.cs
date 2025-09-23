using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IKTMultiTool
{
    public partial class DriverPanel : UserControl
    {
        private TextBox? outputBox;
        public event Action? OnBack;
        private SplitContainer split;
        private string driversBasePath;

        public DriverPanel()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.FromArgb(30, 30, 30);

            driversBasePath = Path.Combine(Application.StartupPath, "drivers");

            split = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical,
                BackColor = Color.FromArgb(30, 30, 30),
                BorderStyle = BorderStyle.FixedSingle,
                IsSplitterFixed = true,
                SplitterDistance = (int)(this.Width * 0.35),
                SplitterWidth = 4
            };

            ShowMainDriverMenu();
        }

        private void ShowMainDriverMenu()
        {
            split.Panel1.Controls.Clear();
            split.Panel2.Controls.Clear();

            var layout = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                BackColor = Color.FromArgb(30, 30, 30),
                Padding = new Padding(0, 20, 12, 20)
            };

            var btnBack = new Button
            {
                Text = "⬅ Tilbake",
                BackColor = Color.FromArgb(45, 45, 45),
                ForeColor = Color.White,
                Font = new Font("Segoe UI Emoji", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(5),
                Size = new Size(350, 40),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 5, 10, 5)
            };
            btnBack.Click += (s, e) => OnBack?.Invoke();
            layout.Controls.Add(btnBack);
            layout.Controls.Add(new Label { Height = 12, Width = 1 });

            var titleLabel = new Label
            {
                Text = "💾 Drivere for PC-modeller",
                AutoSize = false,
                Height = 36,
                ForeColor = Color.White,
                Font = new Font("Segoe UI Emoji", 14, FontStyle.Bold),
                Margin = new Padding(0, 6, 8, 6),
                TextAlign = ContentAlignment.MiddleLeft
            };
            layout.Controls.Add(titleLabel);
            layout.Controls.Add(new Label { Height = 10, Width = 1 });

            if (!Directory.Exists(driversBasePath))
            {
                try
                {
                    Directory.CreateDirectory(driversBasePath);
                    Logger.Log($"DriverPanel: Opprettet drivermappe: {driversBasePath}");
                }
                catch (Exception ex)
                {
                    Logger.Log($"DriverPanel: Feil ved oppretting av drivermappe: {ex.Message}");
                }
            }

            LoadPCModels(layout);

            layout.Controls.Add(new Label { Height = 20, Width = 1 });
            var adminLabel = new Label
            {
                Text = "--- Administrasjon ---",
                AutoSize = false,
                Height = 28,
                ForeColor = Color.LightGray,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Margin = new Padding(0, 6, 8, 6),
                TextAlign = ContentAlignment.MiddleLeft
            };
            layout.Controls.Add(adminLabel);

            var btnAddModel = new Button
            {
                Text = "➕ Legg til ny PC-modell",
                BackColor = Color.FromArgb(45, 45, 45),
                ForeColor = Color.White,
                Font = new Font("Segoe UI Emoji", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(5),
                Size = new Size(90, 80),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 5, 10, 5)
            };
            btnAddModel.Click += (s, e) => ShowAddNewModelDialog();
            layout.Controls.Add(btnAddModel);

            layout.Controls.Add(new Label { Height = 20, Width = 1 });
            var generalLabel = new Label
            {
                Text = "--- Generelle verktøy ---",
                AutoSize = false,
                Height = 28,
                ForeColor = Color.LightGray,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Margin = new Padding(0, 6, 8, 6),
                TextAlign = ContentAlignment.MiddleLeft
            };
            layout.Controls.Add(generalLabel);

            string[] btnTexts = {
                "📂 Åpne hovedmappe for drivere",
                "🔍 Vis installerte drivere",
                "🖥️ Vis maskinvareinfo",
                "🔄 Oppdater alle drivere (Windows Update)",
                "❓ Hjelp - Hvordan legge til drivere"
            };
            Action[] actions = {
                () => OpenDriverFolder(driversBasePath),
                () => RunCmd("driverquery"),
                () => RunCmd("msinfo32"),
                () => RunCmd("powershell -Command \"Get-WindowsUpdate -Category Driver\""),
                () => ShowHelp()
            };
            for (int i = 0; i < btnTexts.Length; i++)
            {
                var btn = new Button
                {
                    Text = btnTexts[i],
                    BackColor = Color.FromArgb(45, 45, 45),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI Emoji", 10, FontStyle.Bold),
                    FlatStyle = FlatStyle.Flat,
                    Margin = new Padding(5),
                    Size = new Size(90, 80),
                    TextAlign = ContentAlignment.MiddleLeft,
                    Padding = new Padding(10, 5, 10, 5)
                };
                int idx = i;
                btn.Click += (s, e) => { Logger.Log($"DriverPanel: Klikket på knapp '{btnTexts[idx]}'"); actions[idx](); };
                layout.Controls.Add(btn);
            }

            outputBox = new TextBox
            {
                Multiline = true,
                Dock = DockStyle.Fill,
                ReadOnly = true,
                BackColor = Color.Black,
                ForeColor = Color.Lime,
                Font = new Font("Consolas", 9),
                ScrollBars = ScrollBars.Vertical,
                BorderStyle = BorderStyle.FixedSingle,
                Text = "Velg en PC-modell for å se tilgjengelige drivere.\n\nDu kan også bruke de generelle verktøyene nedenfor.",
                WordWrap = true
            };

            split.Panel1.Controls.Add(layout);
            split.Panel2.Controls.Add(outputBox);
            if (!this.Controls.Contains(split)) this.Controls.Add(split);

            Color lilla = Color.FromArgb(120, 60, 200);
            outputBox.ForeColor = lilla;
            foreach (var b in layout.Controls.OfType<Button>())
            {
                b.BackColor = lilla;
                b.ForeColor = Color.White;
                b.Height = 60;
                b.Font = new Font("Segoe UI Emoji", 10, FontStyle.Regular);
                b.TextAlign = ContentAlignment.MiddleLeft;
                b.Margin = new Padding(0, 6, 8, 6);
            }
            layout.SizeChanged += (s, e) =>
            {
                int w = Math.Max(100, layout.ClientSize.Width - layout.Padding.Left - layout.Padding.Right);
                foreach (var b in layout.Controls.OfType<Button>()) b.Width = w;
                foreach (var lab in layout.Controls.OfType<Label>()) if (!lab.AutoSize) lab.Width = w;
            };
            ApplyFullWidth(layout);
        }

        private int GetDriverCount(string folderPath)
        {
            try
            {
                if (!Directory.Exists(folderPath)) return 0;
                return Directory.GetFiles(folderPath, "*.*")
                    .Where(f => f.EndsWith(".exe", StringComparison.OrdinalIgnoreCase) ||
                                f.EndsWith(".msi", StringComparison.OrdinalIgnoreCase) ||
                                f.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
                    .Count();
            }
            catch { return 0; }
        }

        private void LoadPCModels(FlowLayoutPanel layout)
        {
            try
            {
                if (!Directory.Exists(driversBasePath))
                {
                    Directory.CreateDirectory(driversBasePath);
                    return;
                }

                var modelDirectories = Directory.GetDirectories(driversBasePath);
                if (modelDirectories.Length == 0)
                {
                    var noModelsLabel = new Label
                    {
                        Text = "⚠️ Ingen PC-modeller funnet. Klikk på 'Legg til ny PC-modell' for å legge til en.",
                        AutoSize = false,
                        Height = 40,
                        ForeColor = Color.Orange,
                        Font = new Font("Segoe UI Emoji", 10),
                        Margin = new Padding(0, 6, 8, 6),
                        TextAlign = ContentAlignment.MiddleLeft
                    };
                    layout.Controls.Add(noModelsLabel);
                    return;
                }

                var pcModelsLabel = new Label
                {
                    Text = "--- Tilgjengelige PC-modeller ---",
                    AutoSize = false,
                    Height = 28,
                    ForeColor = Color.LightGray,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    Margin = new Padding(0, 6, 8, 6),
                    TextAlign = ContentAlignment.MiddleLeft
                };
                layout.Controls.Add(pcModelsLabel);

                foreach (var directory in modelDirectories)
                {
                    var folderName = Path.GetFileName(directory);
                    var displayName = GetDisplayNameFromFolder(folderName);
                    var driverCount = GetDriverCount(directory);
                    var displayText = $"🖥️ {displayName} ({driverCount} drivere)";

                    var btn = new Button
                    {
                        Text = displayText,
                        BackColor = Color.FromArgb(45, 45, 45),
                        ForeColor = Color.White,
                        Font = new Font("Segoe UI Emoji", 10, FontStyle.Bold),
                        FlatStyle = FlatStyle.Flat,
                        Margin = new Padding(5),
                        Size = new Size(90, 80),
                        TextAlign = ContentAlignment.MiddleLeft,
                        Padding = new Padding(10, 5, 10, 5),
                        Tag = folderName
                    };
                    btn.Click += (s, e) => ShowPCDrivers(displayName, folderName);
                    layout.Controls.Add(btn);
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"DriverPanel: Feil ved lasting av PC-modeller: {ex.Message}");
                var errorLabel = new Label
                {
                    Text = "❌ Feil ved lasting av PC-modeller",
                    AutoSize = false,
                    Height = 32,
                    ForeColor = Color.Red,
                    Font = new Font("Segoe UI Emoji", 10),
                    Margin = new Padding(0, 6, 8, 6),
                    TextAlign = ContentAlignment.MiddleLeft
                };
                layout.Controls.Add(errorLabel);
            }
        }

        private string GetDisplayNameFromFolder(string folderName)
        {
            return folderName.Replace("_", " ").Replace("-", " ");
        }

        private void ShowAddNewModelDialog()
        {
            using (var form = new Form())
            {
                form.Text = "Legg til ny PC-modell";
                form.Size = new Size(400, 200);
                form.StartPosition = FormStartPosition.CenterParent;
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.MaximizeBox = false;
                form.MinimizeBox = false;
                form.BackColor = Color.FromArgb(30, 30, 30);

                var label = new Label
                {
                    Text = "Navn på PC-modell:",
                    Location = new Point(20, 20),
                    ForeColor = Color.White,
                    Size = new Size(200, 20),
                    Font = new Font("Segoe UI", 10)
                };

                var textBox = new TextBox
                {
                    Location = new Point(20, 45),
                    Size = new Size(340, 25),
                    Font = new Font("Segoe UI", 10),
                    PlaceholderText = "F.eks: Lenovo Thinkpad E15 Gen5"
                };

                var helpLabel = new Label
                {
                    Text = "Mapper vil opprettes med navn uten mellomrom og spesialtegn.",
                    Location = new Point(20, 75),
                    Size = new Size(340, 40),
                    ForeColor = Color.LightGray,
                    Font = new Font("Segoe UI", 8)
                };

                var btnOK = new Button
                {
                    Text = "Opprett",
                    Location = new Point(200, 120),
                    Size = new Size(75, 30),
                    BackColor = Color.FromArgb(120, 60, 200),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 9, FontStyle.Bold)
                };

                var btnCancel = new Button
                {
                    Text = "Avbryt",
                    Location = new Point(285, 120),
                    Size = new Size(75, 30),
                    BackColor = Color.FromArgb(45, 45, 45),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 9)
                };

                btnOK.Click += (s, e) =>
                {
                    var modelName = textBox.Text.Trim();
                    if (string.IsNullOrEmpty(modelName))
                    {
                        MessageBox.Show("Vennligst skriv inn et navn på PC-modellen.", "Feil", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (CreateNewPCModel(modelName))
                    {
                        form.DialogResult = DialogResult.OK;
                        form.Close();
                        ShowMainDriverMenu();
                    }
                };

                btnCancel.Click += (s, e) =>
                {
                    form.DialogResult = DialogResult.Cancel;
                    form.Close();
                };

                form.Controls.AddRange(new Control[] { label, textBox, helpLabel, btnOK, btnCancel });
                form.ShowDialog();
            }
        }

        private bool CreateNewPCModel(string modelName)
        {
            try
            {
                var folderName = modelName
                    .Replace(" ", "_")
                    .Replace("-", "_")
                    .Replace(".", "_")
                    .Replace(",", "")
                    .Replace("(", "")
                    .Replace(")", "");

                var modelPath = Path.Combine(driversBasePath, folderName);

                if (Directory.Exists(modelPath))
                {
                    MessageBox.Show($"En mappe for '{modelName}' eksisterer allerede.", "Feil", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                Directory.CreateDirectory(modelPath);

                var readmeContent = $@"# {modelName} Drivere

Legg driver-filer (.exe, .msi, .zip) for {modelName} i denne mappen.

Eksempler på drivere du kan legge til:
- Audio_Driver_{folderName}.exe
- WiFi_Driver_{folderName}.zip
- Graphics_Driver_{folderName}.msi
- Bluetooth_Driver_{folderName}.exe
- Touchpad_Driver_{folderName}.exe

Slett denne filen når du har lagt til ekte driver-filer.";

                File.WriteAllText(Path.Combine(modelPath, "LEGG_DRIVERE_HER.txt"), readmeContent);

                Logger.Log($"DriverPanel: Opprettet ny PC-modell: {modelName} (mappe: {folderName})");

                if (outputBox != null)
                {
                    outputBox.Text = $"Opprettet ny PC-modell: {modelName}\nMappe: {modelPath}\n\nKlikk på modellen for å legge til drivere.";
                }

                MessageBox.Show($"PC-modell '{modelName}' ble opprettet!\n\nMappe: {folderName}", "Suksess", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Log($"DriverPanel: Feil ved oppretting av PC-modell: {ex.Message}");
                MessageBox.Show($"Feil ved oppretting av PC-modell: {ex.Message}", "Feil", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void ShowPCDrivers(string pcName, string folderName)
        {
            var driverPath = Path.Combine(driversBasePath, folderName);

            split.Panel1.Controls.Clear();
            split.Panel2.Controls.Clear();

            var layout = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                BackColor = Color.FromArgb(30, 30, 30),
                Padding = new Padding(0, 20, 12, 20)
            };

            var btnBack = new Button
            {
                Text = "⬅ Tilbake til hovedmeny",
                BackColor = Color.FromArgb(45, 45, 45),
                ForeColor = Color.White,
                Font = new Font("Segoe UI Emoji", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(5),
                Size = new Size(350, 40),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 5, 10, 5)
            };
            btnBack.Click += (s, e) => ShowMainDriverMenu();
            layout.Controls.Add(btnBack);
            layout.Controls.Add(new Label { Height = 20, Width = 1 });

            var titleLabel = new Label
            {
                Text = $"💾 {pcName}",
                AutoSize = false,
                Height = 32,
                ForeColor = Color.White,
                Font = new Font("Segoe UI Emoji", 12, FontStyle.Bold),
                Margin = new Padding(0, 6, 8, 6),
                TextAlign = ContentAlignment.MiddleLeft
            };
            layout.Controls.Add(titleLabel);
            layout.Controls.Add(new Label { Height = 10, Width = 1 });

            if (!Directory.Exists(driverPath))
            {
                try
                {
                    Directory.CreateDirectory(driverPath);
                    Logger.Log($"DriverPanel: Opprettet drivermappe: {driverPath}");
                }
                catch (Exception ex)
                {
                    Logger.Log($"DriverPanel: Feil ved oppretting av drivermappe: {ex.Message}");
                }
            }

            LoadDriverButtons(layout, driverPath);

            layout.Controls.Add(new Label { Height = 20, Width = 1 });
            var separatorLabel = new Label
            {
                Text = "--- Verktøy ---",
                AutoSize = false,
                Height = 28,
                ForeColor = Color.LightGray,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Margin = new Padding(0, 6, 8, 6),
                TextAlign = ContentAlignment.MiddleLeft
            };
            layout.Controls.Add(separatorLabel);

            string[] btnTexts = {
                "📂 Åpne drivermappe for denne PC-en",
                "🔍 Vis installerte drivere",
                "🖥️ Vis maskinvareinfo",
                "❓ Hjelp - Hvordan legge til drivere"
            };
            Action[] actions = {
                () => OpenDriverFolder(driverPath),
                () => RunCmd("driverquery"),
                () => RunCmd("msinfo32"),
                () => ShowHelp()
            };
            for (int i = 0; i < btnTexts.Length; i++)
            {
                var btn = new Button
                {
                    Text = btnTexts[i],
                    Size = new Size(90, 80),
                    BackColor = Color.FromArgb(45, 45, 45),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI Emoji", 10, FontStyle.Bold),
                    FlatStyle = FlatStyle.Flat,
                    Margin = new Padding(5),
                    TextAlign = ContentAlignment.MiddleLeft,
                    Padding = new Padding(10, 5, 10, 5)
                };
                int idx = i;
                btn.Click += (s, e) => { Logger.Log($"DriverPanel: Klikket på knapp '{btnTexts[idx]}' for {pcName}"); actions[idx](); };
                layout.Controls.Add(btn);
            }

            this.OnResize(EventArgs.Empty);

            outputBox = new TextBox
            {
                Multiline = true,
                Dock = DockStyle.Fill,
                ReadOnly = true,
                BackColor = Color.Black,
                ForeColor = Color.Lime,
                Font = new Font("Consolas", 9),
                ScrollBars = ScrollBars.Vertical,
                BorderStyle = BorderStyle.FixedSingle,
                Text = $"Drivere for {pcName}\n\nKlikk på en driver for å installere den.",
                WordWrap = true
            };

            split.Panel1.Controls.Add(layout);
            split.Panel2.Controls.Add(outputBox);

            Color lilla2 = Color.FromArgb(120, 60, 200);
            outputBox.ForeColor = lilla2;
            foreach (var b in layout.Controls.OfType<Button>())
            {
                b.BackColor = lilla2;
                b.ForeColor = Color.White;
                b.Height = 60;
                b.Font = new Font("Segoe UI Emoji", 10, FontStyle.Regular);
                b.TextAlign = ContentAlignment.MiddleLeft;
                b.Margin = new Padding(0, 6, 8, 6);
            }
            layout.SizeChanged += (s, e) =>
            {
                int w = Math.Max(100, layout.ClientSize.Width - layout.Padding.Left - layout.Padding.Right);
                foreach (var b in layout.Controls.OfType<Button>()) b.Width = w;
                foreach (var lab in layout.Controls.OfType<Label>()) if (!lab.AutoSize) lab.Width = w;
            };
            ApplyFullWidth(layout);
        }

        private void LoadDriverButtons(FlowLayoutPanel layout, string driverPath)
        {
            try
            {
                if (Directory.Exists(driverPath))
                {
                    var driverFiles = Directory.GetFiles(driverPath, "*.*")
                        .Where(f => f.EndsWith(".exe", StringComparison.OrdinalIgnoreCase) ||
                                    f.EndsWith(".msi", StringComparison.OrdinalIgnoreCase) ||
                                    f.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
                        .ToArray();

                    if (driverFiles.Length > 0)
                    {
                        var driverLabel = new Label
                        {
                            Text = "--- Tilgjengelige drivere ---",
                            AutoSize = false,
                            Height = 28,
                            ForeColor = Color.LightGray,
                            Font = new Font("Segoe UI", 10, FontStyle.Bold),
                            Margin = new Padding(0, 6, 8, 6),
                            TextAlign = ContentAlignment.MiddleLeft
                        };
                        layout.Controls.Add(driverLabel);

                        foreach (var driverFile in driverFiles)
                        {
                            var fileName = Path.GetFileName(driverFile);
                            var btn = new Button
                            {
                                Text = $"📦 {fileName}",
                                BackColor = Color.FromArgb(45, 45, 45),
                                ForeColor = Color.White,
                                Font = new Font("Segoe UI Emoji", 10, FontStyle.Bold),
                                FlatStyle = FlatStyle.Flat,
                                Margin = new Padding(5),
                                Size = new Size(90, 80),
                                TextAlign = ContentAlignment.MiddleLeft,
                                Padding = new Padding(10, 5, 10, 5)
                            };
                            btn.Click += (s, e) => InstallDriver(driverFile);
                            layout.Controls.Add(btn);
                        }
                    }
                    else
                    {
                        var noDriversLabel = new Label
                        {
                            Text = "⚠️ Ingen drivere funnet i mappen",
                            AutoSize = false,
                            Height = 32,
                            ForeColor = Color.Orange,
                            Font = new Font("Segoe UI Emoji", 10),
                            Margin = new Padding(0, 6, 8, 6),
                            TextAlign = ContentAlignment.MiddleLeft
                        };
                        layout.Controls.Add(noDriversLabel);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"DriverPanel: Feil ved lasting av drivere: {ex.Message}");
                var errorLabel = new Label
                {
                    Text = "❌ Feil ved lasting av drivere",
                    AutoSize = false,
                    Height = 32,
                    ForeColor = Color.Red,
                    Font = new Font("Segoe UI Emoji", 10),
                    Margin = new Padding(0, 6, 8, 6),
                    TextAlign = ContentAlignment.MiddleLeft
                };
                layout.Controls.Add(errorLabel);
            }
        }

        private void InstallDriver(string driverPath)
        {
            try
            {
                var fileName = Path.GetFileName(driverPath);
                var fileExtension = Path.GetExtension(driverPath).ToLower();

                if (outputBox != null)
                {
                    outputBox.Text = $"Starter stille installasjon av driver: {fileName}\r\n";
                }
                Logger.Log($"DriverPanel: Starter stille installasjon av driver: {driverPath}");

                ProcessStartInfo psi;

                switch (fileExtension)
                {
                    case ".msi":
                        psi = new ProcessStartInfo
                        {
                            FileName = "msiexec.exe",
                            Arguments = $"/i \"{driverPath}\" /quiet /norestart",
                            UseShellExecute = true,
                            Verb = "runas",
                            CreateNoWindow = true
                        };
                        if (outputBox != null) outputBox.AppendText($"Installerer MSI-pakke i stille modus...\r\n");
                        break;
                    case ".exe":
                        psi = new ProcessStartInfo
                        {
                            FileName = driverPath,
                            Arguments = "/S /silent /quiet /verysilent /norestart",
                            UseShellExecute = true,
                            Verb = "runas",
                            CreateNoWindow = true
                        };
                        if (outputBox != null) outputBox.AppendText($"Installerer EXE med stille parametere...\r\n");
                        break;
                    case ".zip":
                        if (outputBox != null)
                        {
                            outputBox.Text = $"ZIP-filer må pakkes ut manuelt før installasjon.\r\nÅpner mappen hvor filen ligger...\r\n";
                        }
                        var zipFolder = Path.GetDirectoryName(driverPath);
                        if (!string.IsNullOrEmpty(zipFolder))
                        {
                            Process.Start("explorer.exe", $"/select,\"{driverPath}\"");
                        }
                        Logger.Log($"DriverPanel: ZIP-fil åpnet i explorer: {driverPath}");
                        return;
                    default:
                        psi = new ProcessStartInfo
                        {
                            FileName = driverPath,
                            UseShellExecute = true,
                            Verb = "runas"
                        };
                        if (outputBox != null) outputBox.AppendText($"Ukjent filtype, starter vanlig installasjon...\r\n");
                        break;
                }

                var process = Process.Start(psi);
                if (process != null)
                {
                    if (outputBox != null)
                    {
                        outputBox.AppendText($"Installasjon startet (Process ID: {process.Id})\r\n");
                        outputBox.AppendText($"Venter på at installasjonen skal fullføres...\r\n");
                    }

                    Task.Run(() =>
                    {
                        try
                        {
                            process.WaitForExit();
                            var exitCode = process.ExitCode;

                            this.Invoke(new Action(() =>
                            {
                                if (outputBox != null)
                                {
                                    if (exitCode == 0)
                                    {
                                        outputBox.AppendText($"✅ Installasjon fullført med suksess!\r\n");
                                        outputBox.AppendText($"Driver '{fileName}' er nå installert.\r\n");
                                    }
                                    else if (exitCode == 3010)
                                    {
                                        outputBox.AppendText($"⚠️ Installasjon fullført, men krever omstart.\r\n");
                                        outputBox.AppendText($"Restart datamaskinen for å fullføre installasjonen.\r\n");
                                    }
                                    else
                                    {
                                        outputBox.AppendText($"❌ Installasjon fullført med feilkode: {exitCode}\r\n");
                                        outputBox.AppendText($"Dette kan bety at driveren allerede er installert eller at det oppstod en feil.\r\n");
                                    }
                                }
                            }));

                            Logger.Log($"DriverPanel: Driver-installasjon fullført med exit code: {exitCode}");
                        }
                        catch (Exception ex)
                        {
                            this.Invoke(new Action(() =>
                            {
                                if (outputBox != null)
                                {
                                    outputBox.AppendText($"Feil under overvåking av installasjon: {ex.Message}\r\n");
                                }
                            }));
                            Logger.Log($"DriverPanel: Feil under overvåking av installasjon: {ex.Message}");
                        }
                    });
                }
                else
                {
                    if (outputBox != null) outputBox.AppendText($"❌ Kunne ikke starte installasjonsprosessen.\r\n");
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"DriverPanel: Feil ved start av driver-installasjon: {ex.Message}");
                if (outputBox != null)
                {
                    outputBox.Text = $"Feil ved start av installasjon: {ex.Message}\r\n";
                    outputBox.AppendText($"Prøv å kjøre programmet som administrator.\r\n");
                }
            }
        }

        private void OpenDriverFolder(string folderPath)
        {
            try
            {
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                Process.Start("explorer.exe", folderPath);
                if (outputBox != null)
                {
                    outputBox.Text = $"Åpnet drivermappe: {folderPath}\r\n";
                }
                Logger.Log($"DriverPanel: Åpnet drivermappe: {folderPath}");
            }
            catch (Exception ex)
            {
                Logger.Log($"DriverPanel: Feil ved åpning av drivermappe: {ex.Message}");
                if (outputBox != null)
                {
                    outputBox.Text = $"Feil ved åpning av mappe: {ex.Message}\r\n";
                }
            }
        }

        private void OpenDriverFolder()
        {
            OpenDriverFolder(driversBasePath);
        }

        private void ShowHelp()
        {
            var helpText = @"Hvordan legge til drivere:

1. Velg en PC-modell fra hovedmenyen
2. Klikk på 'Åpne drivermappe for denne PC-en' for å åpne mappen hvor drivere skal ligge
3. Last ned drivere fra produsentens nettsider eller andre kilder
4. Kopier driver-filene (.exe, .msi, .zip) til mappen
5. Gå tilbake til hovedmenyen og inn på PC-modellen igjen for å se de nye driverne
6. Klikk på en driver for å installere den

Støttede filtyper:
- .exe (Kjørbare installasjonsfiler)
- .msi (Windows Installer pakker)  
- .zip (Komprimerte arkiver - må pakkes ut manuelt)

Merk: Noen drivere krever administrator-rettigheter for installasjon.";


            if (outputBox != null)
            {
                outputBox.Text = helpText;
            }
            Logger.Log("DriverPanel: Viste hjelp-informasjon");
        }

        private async void RunCmd(string cmd)
        {
            Logger.Log($"DriverPanel: Kjører kommando: {cmd}");
            if (outputBox != null)
            {
                outputBox.Text = "Kjører kommando...\r\n";
            }
            await Task.Run(() =>
            {
                var psi = new ProcessStartInfo("cmd.exe", $"/c {cmd}")
                {
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                try
                {
                    using (var proc = Process.Start(psi))
                    {
                        if (proc != null)
                        {
                            var output = proc.StandardOutput.ReadToEnd();
                            var error = proc.StandardError.ReadToEnd();
                            this.Invoke(new Action(() =>
                            {
                                if (outputBox != null)
                                {
                                    outputBox.Text = string.IsNullOrEmpty(output) ? error : output;
                                    if (!string.IsNullOrEmpty(error) && !string.IsNullOrEmpty(output))
                                        outputBox.AppendText($"\r\nFeil:\r\n{error}");
                                }
                            }));
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.Invoke(new Action(() =>
                    {
                        if (outputBox != null)
                        {
                            outputBox.Text = $"Feil: {ex.Message}";
                        }
                    }));
                    Logger.Log($"DriverPanel: Feil ved kjøring av kommando: {ex.Message}");
                }
            });
        }

        private void ApplyFullWidth(FlowLayoutPanel layout)
        {
            try
            {
                // Force a layout pass to get the final client width
                layout.PerformLayout();
                int w = Math.Max(100, layout.ClientSize.Width - layout.Padding.Left - layout.Padding.Right);
                foreach (var b in layout.Controls.OfType<Button>()) b.Width = w;
                foreach (var lab in layout.Controls.OfType<Label>()) if (!lab.AutoSize) lab.Width = w;
            }
            catch { /* ignore */ }
        }
    }
}
