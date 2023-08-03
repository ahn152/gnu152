using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Windows.Forms.DataVisualization.Charting;
using System.Collections;


namespace MultiColoredModernUI.Forms
{
    public partial class FormDashboard : Form
    {
         string connectionString = "Data Source=222.235.141.8,1111; Initial Catalog=GNU23_04; User ID=GNU2304; Password=1234";

        public FormDashboard()
        {
            InitializeComponent();
        }

        private void FormDashboard_Load(object sender, EventArgs e)
        {


            // 데이터베이스 연결 문자열
            string connectionString = "Data Source=222.235.141.8,1111; Initial Catalog=GNU23_04; User ID=GNU2304; Password=1234";

            #region <달성률 구하기>
            // SQL 쿼리문 
            string query2 = "SELECT SUM(ProductionQuantity) AS TotalProductionQuantity, SUM(ProductionGoal) AS TotalProductionGoal FROM EquipmentPerformance";

            // 달성률 변수
            double achievementRate = 0;

            // SQL Server 연결과 데이터 조회
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(query2, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // 총 생산량과 총 목표량을 가져옴
                            int totalProductionQuantity = Convert.ToInt32(reader["TotalProductionQuantity"]);
                            int totalProductionGoal_ = Convert.ToInt32(reader["TotalProductionGoal"]);

                            // 달성률 계산
                            if (totalProductionGoal_ != 0)
                                achievementRate = (totalProductionQuantity / (double)totalProductionGoal_) * 100;
                        }
                    }
                }
            }
            #endregion

            // DB_label9에 달성률 표시
            DB_label9.Text = achievementRate.ToString("0.00") + "%";

            #region <아래 그리드>
            //DataTable table = new DataTable();

            //table.Columns.Add("작업장", typeof(string));
            //table.Columns.Add("상태", typeof(string));
            //table.Columns.Add("공정정보", typeof(string));
            //table.Columns.Add("설비온도", typeof(string));
            //table.Columns.Add("전압", typeof(string));
            //table.Columns.Add("목표수량(일일)", typeof(string));
            //table.Columns.Add("목표수량(전체)", typeof(string));
            //table.Columns.Add("생산수량", typeof(string));
            //table.Columns.Add("달성률[%]", typeof(string));

            //DB_dataGridView1.DataSource = table;
            #endregion

            #region <주간 목표 수량>
            // 쿼리문 작성
            string query = "SELECT ProductionQuantity, Funiq FROM EquipmentPerformance";

            // 데이터를 담을 DataTable
            DataTable dataTable = new DataTable();

            // SQL Server 연결과 데이터 조회
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(dataTable);
                }
            }
            // 그래프 그리기
            //DrawBarChart(dataTable);

            // SQL 쿼리문
            string query1 = "SELECT ProductionGoal FROM EquipmentPerformance";

            // 합계를 저장할 변수
            int totalProductionGoal = 0;

            // SQL Server 연결과 데이터 조회
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(query1, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // 데이터베이스에서 가져온 값을 더함
                            int productionGoal = reader.GetInt32(0);
                            totalProductionGoal += productionGoal;
                        }
                    }
                }
            }

            // DB_label5에 결과를 표시
            DB_label5.Text = totalProductionGoal.ToString();

            #endregion

            DataTable dt = new DataTable();

           

            DataTable dt220 = GetDataFromDatabase();
            DrawChart(dt220);

           
        }

        private int myPrivateVariable;
       
        public  DataTable GetDataFromDatabase()
        {
            DataTable dt220 = new DataTable();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                    connection.Open();
                string query22 = "SELECT MLINECODE, SUM(WGOOD) TotalWGOOD, SUM(WBAD) TotalWBAD FROM Result GROUP BY MLINECODE;";
                using (SqlCommand command = new SqlCommand(query22, connection))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(dt220);
                    }
                }
            }

            return dt220;
        }

        private void DrawChart(DataTable dataTable)
        {

            
            this.Controls.Add(chart2);

            ChartArea chartArea = new ChartArea();
            chartArea.AxisX.Title = "설비";
            chartArea.AxisY.Title = "Defect Rate (%)";
            chart2.ChartAreas.Add(chartArea);

            Series series = new Series("DefectRate");
            series.ChartType = SeriesChartType.Column;
            chart2.Series.Add(series);

            foreach (DataRow row in dataTable.Rows)
            {
                string equipment = row["MLINECODE"].ToString();
                // int totalProductionQuantity = Convert.ToInt32(row["TotalProductionQuantity"]);
                int TotalWGOOD = Convert.ToInt32(row["TotalWGOOD"]);
                int TotalWBAD = Convert.ToInt32(row["TotalWBAD"]);

                //불량품 수량 / (양품 수량 + 불량품 수량) * 100
                double defectRate = (double)TotalWBAD / (TotalWGOOD+ TotalWBAD) * 100.0;

                chart2.Series["DefectRate"].Points.AddXY(equipment, defectRate);
            }
        }

        
    }

       
    }

          


           

