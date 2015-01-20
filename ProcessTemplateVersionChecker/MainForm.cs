using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProcessTemplateVersionChecker
{
    public partial class MainForm : Form
    {

        private TfsTeamProjectCollection tpc = null;
        private ProjectInfo[] projects = null;
        private ICommonStructureService css = null;
        private ProcessTemplateProperties selectedProjectProps = null;

        public MainForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            TeamProjectPicker tpp = new TeamProjectPicker(TeamProjectPickerMode.MultiProject, false);
            if (tpp.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                tpc = tpp.SelectedTeamProjectCollection;
                projects = tpp.SelectedProjects;
                toolStripStatusLabel1.Text = string.Format("Connected to: [{0}]", tpc.Uri.ToString());
            }

            css = tpc.GetService<ICommonStructureService>();

            FillProjects();
        }

        private void FillProjects()
        {
            listView1.Items.Clear();
            foreach (ProjectInfo p in projects)
            {
                ProcessTemplateProperties ptp = GetProcessTemplateProperties(p);
                ListViewItem lv = new ListViewItem(p.Name);
                lv.SubItems.Add(ptp.CreateVersion);
                lv.SubItems.Add(ptp.CurrentVersion);
                lv.SubItems.Add(ptp.ProcessTemplate);
                lv.Tag = ptp;
                listView1.Items.Add(lv);
            }
        }

        private ProcessTemplateProperties GetProcessTemplateProperties(ProjectInfo pi)
        {
            ProcessTemplateProperties ptp = new ProcessTemplateProperties();
            // Read the properties
            string projectName = string.Empty;
            string projectState = string.Empty;
            int templateId = 0;
            ProjectProperty[] projectProperties = null;

            css.GetProjectProperties(pi.Uri, out projectName, out projectState, out templateId, out projectProperties);

            // Return the properties
            string currentVersion = projectProperties.Where(p => (p.Name == ProcessTemplateProperties.CURRENTVERSIONSTRING)).Select(p => p.Value).FirstOrDefault();
            string createVersion = projectProperties.Where(p => (p.Name == ProcessTemplateProperties.CREATEVERSIONSTRING)).Select(p => p.Value).FirstOrDefault();
            string pt = projectProperties.Where(p => (p.Name == ProcessTemplateProperties.PROCESSTEMPLATESTRING)).Select(p => p.Value).FirstOrDefault();
            
            ptp.CurrentVersion = currentVersion;
            ptp.CreateVersion = createVersion;
            ptp.ProcessTemplate = pt;
            ptp.CurrentProject = pi;

            return ptp;
        }



        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0) { return; }
            ListViewItem lv = listView1.SelectedItems[0];
            lblTP.Text = lv.Text;
            ProcessTemplateProperties ptp = lv.Tag as ProcessTemplateProperties;
            selectedProjectProps = ptp;
            if (ptp != null)
            {
                textBox1.Text = selectedProjectProps.CreateVersion;
                textBox2.Text = selectedProjectProps.CurrentVersion;
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Read the properties
            string projectName = string.Empty;
            string projectState = string.Empty;
            int templateId = 0;
            ProjectProperty[] projectProperties = null;

            css.GetProjectProperties(selectedProjectProps.CurrentProject.Uri, out projectName, out projectState, out templateId, out projectProperties);

            // Update the property to a new value
            List<ProjectProperty> lpp = projectProperties.ToList();


            var createVal = lpp.Where(p => (p.Name == ProcessTemplateProperties.CREATEVERSIONSTRING)).FirstOrDefault();
            if (createVal == null)
            {
                //add a value
                lpp.Add(new ProjectProperty(ProcessTemplateProperties.CREATEVERSIONSTRING, textBox1.Text));

            }
            else
            { 
                //update value
                createVal.Value = textBox1.Text;
            }

            var currentVal = lpp.Where(p => (p.Name == ProcessTemplateProperties.CURRENTVERSIONSTRING)).FirstOrDefault();
            if (currentVal == null)
            {
                //add a value
                lpp.Add(new ProjectProperty(ProcessTemplateProperties.CURRENTVERSIONSTRING, textBox2.Text));

            }
            else
            {
                //update value
                currentVal.Value = textBox2.Text;
            }


            css.UpdateProjectProperties(selectedProjectProps.CurrentProject.Uri, projectState, lpp.ToArray());

            FillProjects();
        }

    }


}
