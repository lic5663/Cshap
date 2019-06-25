using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp
{
    public partial class MainForm : Form
    {
        Random random = new Random(37);
        public MainForm()
        {
            InitializeComponent();

            lvDummy.Columns.Add("Name");
            lvDummy.Columns.Add("Depth");
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            var Fonts = FontFamily.Families;
            foreach (FontFamily font in Fonts)
            {
                cboFont.Items.Add(font.Name);
            }
        }

        void ChangeFont()
        {
            if (cboFont.SelectedIndex < 0)   // cboFont에서 선택한 항목이 없으면 메소드 종료
                return;

            FontStyle style = FontStyle.Regular;    // FontStyle 객체를 초기화

            if (chkBold.Checked)    // "굵게" 체크 박스가 선택되어 있으면 Bold 논리합 수행
                style |= FontStyle.Bold;

            if (chkItalic.Checked)     // "이태릭" 체크박스가 선택되어 있으면 Italic 논리합 수행
                style |= FontStyle.Italic;

            txtSampleText.Font = new Font((string)cboFont.SelectedItem, 10, style);
        }

        private void CboFont_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeFont();
        }

        private void ChkBold_CheckedChanged(object sender, EventArgs e)
        {
            ChangeFont();
        }

        private void ChkItalic_CheckedChanged(object sender, EventArgs e)
        {
            ChangeFont();
        }

        private void TbDummy_Scroll(object sender, EventArgs e)
        {
            pgDummy.Value = tbDummy.Value; // 슬라이더 위치에 따라 프로그레스바의 내용도 변경
        }

        private void BtnModal_Click(object sender, EventArgs e)
        {
            Form frm = new Form();
            frm.Text = "Modal Form";
            frm.Width = 300;
            frm.Height = 300;
            frm.BackColor = Color.Red;
            frm.ShowDialog();   // Modal 창을 띄움. 창이 띄워지는 동안 다른걸 건드릴 수 없다.
        }

        private void BtnModaless_Click(object sender, EventArgs e)
        {
            Form frm = new Form();
            frm.Text = "Modaless Form";
            frm.Width = 300;
            frm.Height = 300;
            frm.BackColor = Color.Green;
            frm.Show();   // Modaless 창을 띄움. 창이 띄워져도 다른걸 건드릴 수 있음
        }

        private void BtnMsgBox_Click(object sender, EventArgs e)
        {
            MessageBox.Show(txtSampleText.Text, "MessageBox Test", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        void TreeToList()
        {
            lvDummy.Items.Clear();
            foreach (TreeNode node in tvDummy.Nodes)
                TreeToList(node);
        }

        void TreeToList(TreeNode Node)
        {
            lvDummy.Items.Add(new ListViewItem(new string[] { Node.Text, Node.FullPath.Count(f => f == '\\').ToString() }));
            foreach (TreeNode node in Node.Nodes)
            {
                TreeToList(node);
            }
        }

        private void BtnAddRoot_Click(object sender, EventArgs e)
        {
            tvDummy.Nodes.Add(random.Next().ToString());
            TreeToList();
        }

        private void BtnAddChild_Click(object sender, EventArgs e)
        {
            if(tvDummy.SelectedNode == null)
            {
                MessageBox.Show("선택된 노드가 없습니다.", "TreeView Test", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            tvDummy.SelectedNode.Nodes.Add(random.Next().ToString());
            tvDummy.SelectedNode.Expand();
            TreeToList();
        }
    }

}
