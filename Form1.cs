using System.Windows.Forms;
using System.Drawing;

namespace IKTMultiTool
{
    public partial class Form1 : Form
    {
        private UserControl? currentPanel;

        public Form1()
        {
            // Logger fjernet for panelvalg
            InitializeComponent();
        }

        private void ShowCategory(string category)
        {
            // Logger fjernet for panelvalg
            if (currentPanel != null)
            {
                this.Controls.Remove(currentPanel);
                currentPanel.Dispose();
                currentPanel = null;
            }
            switch (category)
            {
                case "Nettverk":
                    var netPanel = new NetworkPanel();
                    netPanel.OnBack += ShowMainMenu;
                    currentPanel = netPanel;
                    break;
                case "System":
                    var sysPanel = new SystemPanel();
                    sysPanel.OnBack += ShowMainMenu;
                    currentPanel = sysPanel;
                    break;
                case "Feilsøking":
                case "Feilsøking og vedlikehold":
                case "Feilsoking":
                    var maintenancePanel = new MaintenancePanel();
                    maintenancePanel.OnBack += ShowMainMenu;
                    currentPanel = maintenancePanel;
                    break;
                case "Cache":
                    var officeCachePanel = new OfficeCachePanel();
                    officeCachePanel.OnBack += ShowMainMenu;
                    currentPanel = officeCachePanel;
                    break;
                case "Drivere":
                    var driverPanel = new DriverPanel();
                    driverPanel.OnBack += ShowMainMenu;
                    currentPanel = driverPanel;
                    break;
                default:
                    MessageBox.Show($"Meny for '{category}' er ikke implementert ennå.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
            }
            currentPanel.Dock = DockStyle.Fill;
            this.Controls.Add(currentPanel);
            currentPanel.BringToFront();
        }

        private void ShowMainMenu()
        {
            // Logger fjernet for panelvalg
            if (currentPanel != null)
            {
                this.Controls.Remove(currentPanel);
                currentPanel.Dispose();
                currentPanel = null;
            }
            // Hovedmeny vises automatisk fordi den ligger i mainPanel
        }
    }
}
