﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using GitCommands;
using GitUI.HelperDialogs;
using ResourceManager.Translation;

namespace GitUI
{
    public partial class BranchComboBox : GitExtensionsControl
    {
        private readonly TranslationString _branchCheckoutError = new TranslationString("Branch '{0}' is not selectable, this branch has been removed from the selection.");
        
        public BranchComboBox()
        {
            InitializeComponent();
            Translate();

            branches.DisplayMember = "Name";
        }

        private IList<GitHead> _branchesToSelect;
        public IList<GitHead> BranchesToSelect
        {
            get
            {
                return _branchesToSelect;
            }
            set
            {
                _branchesToSelect = value;
                LoadBranches();
            }
        }

        private void LoadBranches()
        {
            if (_branchesToSelect != null)
                branches.Items.AddRange(_branchesToSelect.ToArray());
        }

        public IEnumerable<GitHead> GetSelectedBranches()
        {
            foreach (string branch in branches.Text.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries))
            {
                GitHead gitHead = _branchesToSelect.FirstOrDefault(g => g.Name == branch);
                if (gitHead == null)
                    MessageBox.Show(string.Format(_branchCheckoutError.Text, branch));
                else
                    yield return gitHead;
            }
        }

        public string GetSelectedText()
        {
            return branches.Text;
        }

        public void SetSelectedText(string text)
        {
            if (text == null)
                throw new ArgumentNullException("text");

            branches.Text = text;
        }

        private void selectMultipleBranchesButton_Click(object sender, EventArgs e)
        {
            using (FormSelectMultipleBranches formSelectMultipleBranches = new FormSelectMultipleBranches(_branchesToSelect))
            {
                foreach (GitHead branch in GetSelectedBranches())
                    formSelectMultipleBranches.SelectBranch(branch.Name);
                formSelectMultipleBranches.ShowDialog(this);
                string branchesText = string.Empty;
                foreach (GitHead branch in formSelectMultipleBranches.GetSelectedBranches())
                {
                    if (!string.IsNullOrEmpty(branchesText))
                        branchesText += " ";
                    branchesText += branch.Name;
                }

                branches.Text = branchesText;
            }
        }
    }
}
