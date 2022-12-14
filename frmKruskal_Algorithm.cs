using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Kruskal_s_algorithm
{
    public partial class frmKruskal_Algorithm : XtraForm
    {
        public frmKruskal_Algorithm()
        {
            InitializeComponent();
        }
        #region Member Variables
        const int m_nRadius = 20;
        const int m_nHalfRadius = (m_nRadius / 2);

        Color m_colVertex = Color.Aqua;
        Color m_colEdge = Color.Red;

        List<Node> m_lstVertices;
        List<Link> m_lstEdgesInitial, m_lstEdgesFinal;

        Node m_vFirstVertex, m_vSecondVertex;

        bool m_bDrawEdge, m_bSolved;

        #endregion
        #region Events
        private void btnSolve_Click(object sender, System.EventArgs e)
        {
            if (m_lstVertices.Count > 2)
            {
                if (m_lstEdgesInitial.Count < m_lstVertices.Count - 1)
                {
                    XtraMessageBox.Show("Missing Edges", "Alert", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else
                {
                    btnSolve.Enabled = false;
                    int nTotalCost = 0;
                    m_lstEdgesFinal = SolveGraph(ref nTotalCost);
                    m_bSolved = true;
                    panelKruskal.Invalidate();
                    XtraMessageBox.Show("Total Cost : " + nTotalCost.ToString(), "Solution", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
        private void btnClear_Click(object sender, System.EventArgs e)
        {
            DialogResult dr = XtraMessageBox.Show("Clear form ?", "Alert", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (dr == DialogResult.Yes)
            {
                btnSolve.Enabled = true;
                Graphics g = panelKruskal.CreateGraphics();
                g.Clear(panelKruskal.BackColor);
                Reset();
            }
        }
        private void panelKruskal_MouseClick(object sender, MouseEventArgs e)
        {
            Point pClicked = new Point(e.X - m_nHalfRadius, e.Y - m_nHalfRadius);
            if (Control.ModifierKeys == Keys.Control)//if Ctrl is pressed
            {
                if (!m_bDrawEdge)
                {
                    m_vFirstVertex = GetSelectedVertex(pClicked);
                    m_bDrawEdge = true;
                }
                else
                {
                    m_vSecondVertex = GetSelectedVertex(pClicked);
                    m_bDrawEdge = false;
                    if (m_vFirstVertex != null && m_vSecondVertex != null && m_vFirstVertex.Name != m_vSecondVertex.Name)
                    {
                        frmCost formCost = new frmCost();
                        formCost.ShowDialog();

                        Point pStringPoint = GetStringPoint(m_vFirstVertex.pPosition, m_vSecondVertex.pPosition);
                        m_lstEdgesInitial.Add(new Link(m_vFirstVertex, m_vSecondVertex, formCost.m_nCost, pStringPoint));
                        panelKruskal.Invalidate();
                    }
                }
            }
            else
            {
                m_lstVertices.Add(new Node(m_lstVertices.Count, pClicked));
                panelKruskal.Invalidate();
            }
        }
        private void panelKruskal_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            DrawVertices(g);
            DrawEdges(g);
            g.Dispose();
        }
        #endregion
        #region Methods
        #region Drawing
        private void DrawEdges(Graphics g)
        {
            Pen P = new Pen(m_colEdge);
            List<Link> lstEdges = m_bSolved ? m_lstEdgesFinal : m_lstEdgesInitial;
            foreach (Link e in lstEdges)
            {
                Point pV1 = new Point(e.V1.pPosition.X + m_nHalfRadius, e.V1.pPosition.Y + m_nHalfRadius);
                Point pV2 = new Point(e.V2.pPosition.X + m_nHalfRadius, e.V2.pPosition.Y + m_nHalfRadius);
                g.DrawLine(P, pV1, pV2);
                DrawString(e.Cost.ToString(), e.StringPosition, g);
            }
        }
        private void DrawString(string strText, Point pDrawPoint, Graphics g)
        {
            Font drawFont = new Font("Arial", 15);
            SolidBrush drawBrush = new SolidBrush(Color.Black);
            g.DrawString(strText, drawFont, drawBrush, pDrawPoint);
        }
        private void DrawVertices(Graphics g)
        {
            Pen P = new Pen(m_colVertex);
            Brush B = new SolidBrush(m_colVertex);
            foreach (Node v in m_lstVertices)
            {
                g.DrawEllipse(P, v.pPosition.X, v.pPosition.Y, m_nRadius, m_nRadius);
                g.FillEllipse(B, v.pPosition.X, v.pPosition.Y, m_nRadius, m_nRadius);
                DrawString(v.Name.ToString(), v.pPosition, g);
            }
        }
        private Node GetSelectedVertex(Point pClicked)
        {
            Node vReturn = null;
            double dDistance;
            foreach (Node v in m_lstVertices)
            {
                dDistance = GetDistance(v.pPosition, pClicked);
                if (dDistance <= m_nRadius)
                {
                    vReturn = v;
                    break;
                }
            }
            return vReturn;
        }
        private double GetDistance(Point pStart, Point pFinish)
        {
            return Math.Sqrt(Math.Pow(pStart.X - pFinish.X, 2) + Math.Pow(pStart.Y - pFinish.Y, 2));
        }
        private void frmKruskal_Algorithm_Load(object sender, EventArgs e)
        {
            btnSolve.Enabled = true;
            Graphics g = panelKruskal.CreateGraphics();
            g.Clear(panelKruskal.BackColor);
            Reset();
        }
        private Point GetStringPoint(Point pStart, Point pFinish)
        {
            int X = (pStart.X + pFinish.X) / 2;
            int Y = (pStart.Y + pFinish.Y) / 2;
            return new Point(X, Y);
        }
        #endregion
        private void Reset()
        {
            m_lstVertices = new List<Node>();
            m_lstEdgesInitial = new List<Link>();
            m_bSolved = false;
            m_vFirstVertex = m_vSecondVertex = null;
        }
        private List<Link> SolveGraph(ref int nTotalCost)
        {
            Link.QuickSort(m_lstEdgesInitial, 0, m_lstEdgesInitial.Count - 1);
            List<Link> lstEdgesRetun = new List<Link>(m_lstEdgesInitial.Count);
            foreach (Link ed in m_lstEdgesInitial)
            {
                Node vRoot1, vRoot2;
                vRoot1 = ed.V1.GetRoot();
                vRoot2 = ed.V2.GetRoot();
                if (vRoot1.Name != vRoot2.Name)
                {
                    nTotalCost += ed.Cost;
                    Node.Join(vRoot1, vRoot2);
                    lstEdgesRetun.Add(new Link(ed.V1, ed.V2, ed.Cost, ed.StringPosition));
                }
            }
            return lstEdgesRetun;
        }
        #endregion
    }
}
