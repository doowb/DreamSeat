﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LoveSeat;
using MindTouch.Tasking;

namespace ContactManager
{
	public delegate void ContactChangedDelegate(object sender, Contact aContact);

	public partial class ContactDetails : UserControl
	{
		private Contact theContact;
		private CouchDatabase theDatabase;

		public event ContactChangedDelegate ContactChanged;

		public ContactDetails()
		{
			InitializeComponent();
		}

		public CouchDatabase Database
		{
			get { return theDatabase; }
			set
			{
				theDatabase = value;
				theSaveButton.Enabled = true;
			}
		}

		public Contact CurrentContact
		{
			get { return theContact; }
			set
			{
				theContact = value;
				RefreshContact();
			}
		}

		private void RefreshContact()
		{
			if(theContact != null)
			{
				theFirstNameTextBox.Text = theContact.FirstName;
				theLastNameTextBox.Text = theContact.LastName;
				StringBuilder emails = new StringBuilder();
				foreach(var email in theContact.EmailAddresses)
				{
					emails.AppendLine(email);
				}
				theEmailsTextBox.Text = emails.ToString();
			}
			else
			{
				theFirstNameTextBox.Text = String.Empty;
				theLastNameTextBox.Text = String.Empty;
				theEmailsTextBox.Text = String.Empty;
			}
		}

		private void theSaveButton_Click(object sender, EventArgs e)
		{
			bool isNew = theContact == null;
			if(isNew)
			{
				theContact = new Contact();
			}

			theContact.FirstName = theFirstNameTextBox.Text;
			theContact.LastName = theLastNameTextBox.Text;
			theContact.EmailAddresses.Clear();
			foreach (var email in theEmailsTextBox.Text.Split('\n'))
			{
				theContact.EmailAddresses.Add(email.Trim());
			}

			if(isNew)
			{
				theContact = Database.CreateDocument(theContact, new Result<Contact>()).Wait();
			}
			else
			{
				theContact = Database.UpdateDocument(theContact, new Result<Contact>()).Wait();
			}

			if (ContactChanged != null)
				ContactChanged(this, theContact);
		}
	}
}