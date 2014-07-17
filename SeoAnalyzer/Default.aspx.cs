﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Net;
using HtmlAgilityPack;

namespace SeoAnalyzer
{
    public partial class MainForm : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void StartBtn_Click(object sender, EventArgs e)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument htmlDoc = new HtmlDocument();
            string input_text = "";
            if (RadioButtonList1.Items[1].Selected)
            {
                string Url = Input.Text;
                if (!Url.ToLower().StartsWith("http://") && !Url.ToLower().StartsWith("https://"))
                {
                    Url = "http://" + Url;
                }
                htmlDoc = web.Load(Url);
                input_text = HtmlAnalyzer.GetTextFromHtml(htmlDoc);
            }

            if (RadioButtonList1.Items[0].Selected)
            {
                input_text = Input.Text;
            }


            Dictionary<string, int> words = new Dictionary<string, int>();
            words = Filter.GetDict(input_text);

            //Filter stop words
            if (CheckBoxList1.Items[0].Selected)
            {
                Filter.RemoveStopWords(words);
            }

            //Word counts
            if (CheckBoxList1.Items[1].Selected)
            {
                GridView1.Visible = true;
                GridView1.DataSource = words.ToList();
                GridView1.DataBind();
            }
            else
            {
                GridView1.Visible = false;
            }

            //Words in meta tags
            GridView2.Visible = false;
            if ((CheckBoxList1.Items[2].Selected) && (RadioButtonList1.Items[1].Selected))
            {
                Dictionary<string, int> meta_words = new Dictionary<string, int>();
                meta_words = HtmlAnalyzer.GetMetaWords(words, htmlDoc);
                if (meta_words.Count != 0)
                {
                    GridView2.Visible = true;
                    GridView2.DataSource = meta_words.ToList();
                    GridView2.DataBind();
                }
                else
                {
                    GridView2.Visible = false;
                    Label3.Text = "No meta keywords";
                }
            }

            //External links count
            if ((CheckBoxList1.Items[3].Selected) && (RadioButtonList1.Items[1].Selected))
            {
                Label4.Visible = true;
                Label4.Text = "Number of links: " + HtmlAnalyzer.GetCount(htmlDoc).ToString();
            }
            else
            {
                Label4.Visible = false;
            }
        }


        public string GridViewSortDirection(string column)
        {
            string sortDirection = "ASC";
            string lastDirection = ViewState["SortDirection"] as string;
            if (lastDirection == "ASC")
                {
                    sortDirection = "DESC";
                }
            ViewState["SortDirection"] = sortDirection;
            return sortDirection;
        }

        protected void GridView1_Sorting(object sender, GridViewSortEventArgs e)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Key");
            dt.Columns.Add("Value", typeof(int));

            foreach (GridViewRow row in GridView1.Rows)
            {
                DataRow dr;
                dr = dt.NewRow();

                for (int i = 0; i < row.Cells.Count; i++)
                {
                    dr[i] = row.Cells[i].Text;
                }
                dt.Rows.Add(dr);
            }

            if (dt != null)
            {
                dt.DefaultView.Sort = e.SortExpression + " " + GridViewSortDirection(e.SortExpression);
                GridView1.DataSource = dt;
                GridView1.DataBind();
            }
        }

        protected void GridView2_Sorting(object sender, GridViewSortEventArgs e)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Key");
            dt.Columns.Add("Value", typeof(int));

            foreach (GridViewRow row in GridView2.Rows)
            {
                DataRow dr;
                dr = dt.NewRow();

                for (int i = 0; i < row.Cells.Count; i++)
                {
                    dr[i] = row.Cells[i].Text;
                }
                dt.Rows.Add(dr);
            }

            if (dt != null)
            {
                dt.DefaultView.Sort = e.SortExpression + " " + GridViewSortDirection(e.SortExpression);
                GridView2.DataSource = dt;
                GridView2.DataBind();
            }
        }

    }
}