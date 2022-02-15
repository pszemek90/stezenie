using System;
using System.Windows.Forms;
using System.Collections;
using System.Numerics;
using Tekla.Structures.Model;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model.UI;
using Tekla.Structures.Model.Operations;

namespace Exercise
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            MyModel = new Model();
            comboBox1.SelectedIndex = comboBox1.Items.IndexOf("D20");
            comboBox2.SelectedIndex = comboBox2.Items.IndexOf("4017-8.8");
            comboBox3.SelectedIndex = comboBox3.Items.IndexOf("20");
        }

        private readonly Model MyModel;


        private void CreatePadFootings(object sender, EventArgs e)
        {
            // Always remember to check that you really have working connection
            if (MyModel.GetConnectionStatus())
            {
                double wsp3 = int.Parse(textBox1.Text);
                double wsp4 = double.Parse(textBox2.Text);
                
                Picker piku = new Picker();


                //askjdhygausgfdyzjusrgfafsa
                ArrayList info = new ArrayList();
                info = new ArrayList { "END_X", "END_Y", "END_Z", "START_X", "START_Y", "START_Z" };
                ArrayList info2 = new ArrayList();
                info2 = new ArrayList { "END_X", "END_Y", "END_Z", "START_X", "START_Y", "START_Z" };
                ModelObject part = new Beam();
                ModelObject part2 = new Beam();
                Hashtable p1 = new Hashtable();
                Hashtable p2 = new Hashtable();


                Point st1 = new Point();
                Point st2 = new Point();
                try
                {
                    part = piku.PickObject(Picker.PickObjectEnum.PICK_ONE_PART);
                    part2 = piku.PickObject(Picker.PickObjectEnum.PICK_ONE_PART);
                    st1 = piku.PickPoint();
                    st2 = piku.PickPoint();
                }
                catch (Exception picker_fail)
                {
                    MessageBox.Show("Przerwano polecenie");
                }

                part.GetDoubleReportProperties(info, ref p1);
                part2.GetDoubleReportProperties(info2, ref p2);               
                Point st3 = new Point(st1);
                Point st4 = new Point(st2);

                //wektory belek
                Vector v1 = new Vector((Convert.ToDouble(p1["START_X"]) - Convert.ToDouble(p1["END_X"])) / 10000, (Convert.ToDouble(p1["START_Y"]) - Convert.ToDouble(p1["END_Y"])) / 10000, (Convert.ToDouble(p1["START_Z"]) - Convert.ToDouble(p1["END_Z"])) / 10000);
                Vector v2 = new Vector((Convert.ToDouble(p2["START_X"]) - Convert.ToDouble(p2["END_X"])) / 10000, (Convert.ToDouble(p2["START_Y"]) - Convert.ToDouble(p2["END_Y"])) / 10000, (Convert.ToDouble(p2["START_Z"]) - Convert.ToDouble(p2["END_Z"])) / 10000);
                double v1d = v1.GetLength(); double ile1 = 30 / v1d;
                double v2d = v2.GetLength(); double ile2 = 30 / v2d;
                double v1dd = v1.GetLength(); double ile3 = wsp4 / v1d;
                double v2dd = v2.GetLength(); double ile4 = wsp4 / v2d;

                st3.Translate((-v1.X) * ile3, (-v1.Y) * ile3, (-v1.Z) * ile3);
                st1.Translate((v1.X) * ile1, (v1.Y) * ile1, (v1.Z) * ile1);
                st4.Translate((-v2.X) * ile2, (-v2.Y) * ile2, (-v2.Z) * ile2);
                st2.Translate((v2.X) * ile4, (v2.Y) * ile4, (v2.Z) * ile4);

                Contour testowy = new Contour();
                ContourPoint point1 = new ContourPoint(st1, null);
                ContourPoint point2 = new ContourPoint(st3, new Chamfer(45, 0, Chamfer.ChamferTypeEnum.CHAMFER_ROUNDING));
                ContourPoint point3 = new ContourPoint(st4, null);
                testowy.AddContourPoint(point1);
                testowy.AddContourPoint(point2);
                testowy.AddContourPoint(point3);

                Contour testowy3 = new Contour();
                ContourPoint point10 = new ContourPoint(st4, null);
                ContourPoint point20 = new ContourPoint(st2, new Chamfer(45, 0, Chamfer.ChamferTypeEnum.CHAMFER_ROUNDING));
                ContourPoint point30 = new ContourPoint(st3, null);
                testowy3.AddContourPoint(point10);
                testowy3.AddContourPoint(point20);
                testowy3.AddContourPoint(point30);

                string strona = "1";
                if (radioButton1.Checked == true)
                    strona = "1";
                if (radioButton2.Checked == true)
                    strona = "2";
                if (radioButton3.Checked == true)
                    strona = "3";
                if (radioButton4.Checked == true)
                    strona = "4";

                ModelObject nb = CreateBlacha(testowy, strona);
                ModelObject nb3 = CreateBlacha2(testowy3, strona);
                nb.Insert();
                nb3.Insert();

                double l1 = 0.0;
                nb.GetReportProperty("LENGTH", ref l1);
                

                double l2 = 0.0;
                nb3.GetReportProperty("LENGTH", ref l2);

                nb.Delete();
                nb3.Delete();

                

                Contour testowy2 = new Contour();
                testowy2.AddContourPoint(point1);
                testowy2.AddContourPoint(point2);
                Point point4v = new Point(dlugosc(point2, point3, l1));
                ContourPoint point4 = new ContourPoint(point4v, null);
                testowy2.AddContourPoint(point4);
                ModelObject nb2 = CreateBlacha(testowy2, strona);

                Contour testowy4 = new Contour();
                testowy4.AddContourPoint(point10);
                testowy4.AddContourPoint(point20);
                Point point40v = new Point(dlugosc(point20, point30, l2));
                ContourPoint point40 = new ContourPoint(point40v, null);
                testowy4.AddContourPoint(point40);
                ModelObject nb4 = CreateBlacha2(testowy4, strona);

               
                nb2.Insert();
                BoltArray b2 = sruby(point1, point2, nb2 as Part);
                b2.PartToBoltTo = part as Part;
                b2.BoltSize = Convert.ToDouble(comboBox3.Text);
                b2.BoltStandard = comboBox2.Text;
                b2.Insert();
                nb4.Insert();
                BoltArray b4 = sruby(point10, point20, nb4 as Part);
                b4.BoltSize = Convert.ToDouble(comboBox3.Text);
                b4.BoltStandard = comboBox2.Text;
                b4.PartToBoltTo = part2 as Part;
                b4.Insert();

                Point point4st = new Point(dlugosc(point40, point4, 130));
                Point point40st = new Point(dlugosc(point4, point40, 130));

                Point pointsplit = new Point(dlugosc(point4st, point40st, wsp3 + 180));

                if (comboBox1.Text == "D16")
                {
                    Beam st16 = CreateStezenie16(point4st, point40st, strona);
                    st16.Insert();
                    Weld w = new Weld();
                    Weld w2 = new Weld();
                    w.MainObject = st16;
                    w.SecondaryObject = nb2;
                    w.ShopWeld = true;
                    w.Insert();
                    w2.MainObject = st16;
                    w2.SecondaryObject = nb4;
                    w2.ShopWeld = true;
                    w2.Insert();
                    Beam st161 = Operation.Split(st16, pointsplit);
                    Connection sr = new Connection();
                    sr.Name = "Po³¹czenie œrub¹ rzymsk¹";
                    sr.Number = 126;
                    sr.LoadAttributesFromFile("82269_M16");
                    sr.SetPrimaryObject(st161);
                    sr.SetSecondaryObject(st16);
                    sr.Insert();

                    Assembly assembly = st161.GetAssembly();
                    assembly.AssemblyNumber.StartNumber = 1;
                    assembly.Name = "STÊ¯ENIE";
                    assembly.AssemblyNumber.Prefix = "ST";
                    assembly.Modify();

                }
                if(comboBox1.Text == "D20")
                {
                    Beam st16 = CreateStezenie20(point4st, point40st, strona);
                    st16.Insert();
                    Weld w = new Weld();
                    Weld w2 = new Weld();
                    w.MainObject = st16;
                    w.SecondaryObject = nb2;
                    w.ShopWeld = true;
                    w.Insert();
                    w2.MainObject = st16;
                    w2.SecondaryObject = nb4;
                    w2.ShopWeld = true;
                    w2.Insert();
                    Beam st161 = Operation.Split(st16, pointsplit);
                    Connection sr = new Connection();
                    sr.Name = "Po³¹czenie œrub¹ rzymsk¹";
                    sr.Number = 126;
                    sr.LoadAttributesFromFile("82269_M20");
                    sr.SetPrimaryObject(st161);
                    sr.SetSecondaryObject(st16);
                    sr.Insert();

                    Assembly assembly = st161.GetAssembly();
                    assembly.AssemblyNumber.StartNumber = 1;
                    assembly.Name = "STÊ¯ENIE";
                    assembly.AssemblyNumber.Prefix = "ST";
                    assembly.Modify();

                }
                if (comboBox1.Text == "D12")
                {
                    Beam st16 = CreateStezenie12(point4st, point40st, strona);
                    st16.Insert();
                    Weld w = new Weld();
                    Weld w2 = new Weld();
                    w.MainObject = st16;
                    w.SecondaryObject = nb2;
                    w.ShopWeld = true;
                    w.Insert();
                    w2.MainObject = st16;
                    w2.SecondaryObject = nb4;
                    w2.ShopWeld = true;
                    w2.Insert();
                    Beam st161 = Operation.Split(st16, pointsplit);
                    Connection sr = new Connection();
                    sr.Name = "Po³¹czenie œrub¹ rzymsk¹";
                    sr.Number = 126;
                    sr.LoadAttributesFromFile("82269_M12");
                    sr.SetPrimaryObject(st161);
                    sr.SetSecondaryObject(st16);
                    sr.Insert();

                    Assembly assembly = st161.GetAssembly();
                    assembly.AssemblyNumber.StartNumber = 1;
                    assembly.Name = "STÊ¯ENIE";
                    assembly.AssemblyNumber.Prefix = "ST";
                    assembly.Modify();
                }

                if (comboBox1.Text == "D24")
                {
                    Beam st24 = CreateStezenie24(point4st, point40st, strona);
                    st24.Insert();
                    Weld w = new Weld();
                    Weld w2 = new Weld();
                    w.MainObject = st24;
                    w.SecondaryObject = nb2;
                    w.ShopWeld = true;
                    w.Insert();
                    w2.MainObject = st24;
                    w2.SecondaryObject = nb4;
                    w2.ShopWeld = true;
                    w2.Insert();
                    Beam st161 = Operation.Split(st24, pointsplit);
                    Connection sr = new Connection();
                    sr.Name = "Po³¹czenie œrub¹ rzymsk¹";
                    sr.Number = 126;
                    sr.LoadAttributesFromFile("82269_M24");
                    sr.SetPrimaryObject(st161);
                    sr.SetSecondaryObject(st24);
                    sr.Insert();

                    Assembly assembly = st161.GetAssembly();
                    assembly.AssemblyNumber.StartNumber = 1;
                    assembly.Name = "STÊ¯ENIE";
                    assembly.AssemblyNumber.Prefix = "ST";
                    assembly.Modify();

                }

            }
            MyModel.CommitChanges();

        }

        private static ModelObject CreateBlacha(Contour testowy, string strona)
        {
            PolyBeam blacha = new PolyBeam();

          
            blacha.Contour = testowy;
            blacha.Name = "BLACHA";
            blacha.Profile.ProfileString = "BL15*60";
            blacha.Material.MaterialString = "S355JR";
            blacha.Class = "2";
            blacha.PartNumber.Prefix = "BL";
            blacha.PartNumber.StartNumber = 1001;
            if (strona == "1")
            {
                blacha.Position.Plane = Position.PlaneEnum.RIGHT;
            }
            if (strona == "2")
            {
                blacha.Position.Plane = Position.PlaneEnum.LEFT;
            }
            if (strona == "3")
            {
                blacha.Position.Plane = Position.PlaneEnum.LEFT;
            }
            if (strona == "4")
            {
                blacha.Position.Plane = Position.PlaneEnum.RIGHT;
            }
            blacha.Position.Depth = Position.DepthEnum.MIDDLE;

            return blacha;
        }

        private static ModelObject CreateBlacha2(Contour testowy, string strona)
        {
            PolyBeam blacha = new PolyBeam();


            blacha.Contour = testowy;
            blacha.Name = "BLACHA";
            blacha.Profile.ProfileString = "BL15*60";
            blacha.Material.MaterialString = "S355JR";
            blacha.Class = "2";
            blacha.PartNumber.Prefix = "BL";
            blacha.PartNumber.StartNumber = 1001;
            if (strona == "1")
            {
                blacha.Position.Plane = Position.PlaneEnum.RIGHT;
            }
            if (strona == "2")
            {
                blacha.Position.Plane = Position.PlaneEnum.LEFT;
            }
            if (strona == "3")
            {
                blacha.Position.Plane = Position.PlaneEnum.RIGHT;
            }
            if (strona == "4")
            {
                blacha.Position.Plane = Position.PlaneEnum.LEFT;
            }
            blacha.Position.Depth = Position.DepthEnum.MIDDLE;

            return blacha;
        }

        private static Beam CreateStezenie16(Point x, Point y, string strona)
        {
            Beam belka = new Beam();

           
            belka.StartPoint = x;
            belka.EndPoint = y;
            belka.Name = "PROFIL";
            belka.Profile.ProfileString = "D16";
            belka.Material.MaterialString = "S355JR";
            belka.Class = "2";
            belka.Position.Plane = Position.PlaneEnum.MIDDLE;
            belka.Position.Depth = Position.DepthEnum.MIDDLE;
            belka.PartNumber.Prefix = "Pr";
            belka.PartNumber.StartNumber = 1001;
            belka.AssemblyNumber.Prefix = "";
            belka.AssemblyNumber.StartNumber = 0;
            if (strona == "1")
            {
                belka.StartPointOffset.Dy = -23;
                belka.EndPointOffset.Dy = 23;
            }
            if (strona == "2")
            {
                belka.StartPointOffset.Dy = 23;
                belka.EndPointOffset.Dy = -23;
            }

            if (strona == "3")
            {
                belka.StartPointOffset.Dy = 23;
                belka.EndPointOffset.Dy = 23;
            }
            if (strona == "4")
            {
                belka.StartPointOffset.Dy = -23;
                belka.EndPointOffset.Dy = -23;
            }

            return belka;
        }

        private static Beam CreateStezenie20(Point x, Point y, string strona)
        {
            Beam belka = new Beam();


            belka.StartPoint = x;
            belka.EndPoint = y;
            belka.Name = "PROFIL";
            belka.Profile.ProfileString = "D20";
            belka.Material.MaterialString = "S355JR";
            belka.Class = "2";
            belka.Position.Plane = Position.PlaneEnum.MIDDLE;
            belka.Position.Depth = Position.DepthEnum.MIDDLE;
            belka.PartNumber.Prefix = "Pr";
            belka.PartNumber.StartNumber = 1001;
            belka.AssemblyNumber.Prefix = "";
            belka.AssemblyNumber.StartNumber = 0;
            if (strona == "1")
            {
                belka.StartPointOffset.Dy = -25;
                belka.EndPointOffset.Dy = 25;
            }
            if (strona == "2")
            {
                belka.StartPointOffset.Dy = 25;
                belka.EndPointOffset.Dy = -25;
            }
            if (strona == "3")
            {
                belka.StartPointOffset.Dy = 25;
                belka.EndPointOffset.Dy = 25;
            }
            if (strona == "4")
            {
                belka.StartPointOffset.Dy = -25;
                belka.EndPointOffset.Dy = -25;
            }

            return belka;
        }

        private static Beam CreateStezenie24(Point x, Point y, string strona)
        {
            Beam belka = new Beam();


            belka.StartPoint = x;
            belka.EndPoint = y;
            belka.Name = "PROFIL";
            belka.Profile.ProfileString = "D24";
            belka.Material.MaterialString = "S355JR";
            belka.Class = "2";
            belka.Position.Plane = Position.PlaneEnum.MIDDLE;
            belka.Position.Depth = Position.DepthEnum.MIDDLE;
            belka.PartNumber.Prefix = "Pr";
            belka.PartNumber.StartNumber = 1001;
            belka.AssemblyNumber.Prefix = "";
            belka.AssemblyNumber.StartNumber = 0;
            if (strona == "1")
            {
                belka.StartPointOffset.Dy = -27;
                belka.EndPointOffset.Dy = 27;
            }
            if (strona == "2")
            {
                belka.StartPointOffset.Dy = 27;
                belka.EndPointOffset.Dy = -27;
            }
            if (strona == "3")
            {
                belka.StartPointOffset.Dy = 27;
                belka.EndPointOffset.Dy = 27;
            }
            if (strona == "4")
            {
                belka.StartPointOffset.Dy = -27;
                belka.EndPointOffset.Dy = -27;
            }

            return belka;
        }

        private static Beam CreateStezenie12(Point x, Point y, string strona)
        {
            Beam belka = new Beam();


            belka.StartPoint = x;
            belka.EndPoint = y;
            belka.Name = "PROFIL";
            belka.Profile.ProfileString = "D12";
            belka.Material.MaterialString = "S355JR";
            belka.Class = "2";
            belka.Position.Plane = Position.PlaneEnum.MIDDLE;
            belka.Position.Depth = Position.DepthEnum.MIDDLE;
            belka.PartNumber.Prefix = "Pr";
            belka.PartNumber.StartNumber = 1001;
            belka.AssemblyNumber.Prefix = "";
            belka.AssemblyNumber.StartNumber = 0;
            if (strona == "1")
            {
                belka.StartPointOffset.Dy = -21;
                belka.EndPointOffset.Dy = 21;
            }
            if (strona == "2")
            {
                belka.StartPointOffset.Dy = 21;
                belka.EndPointOffset.Dy = -21;
            }
            if (strona == "3")
            {
                belka.StartPointOffset.Dy = 21;
                belka.EndPointOffset.Dy = 21;
            }
            if (strona == "4")
            {
                belka.StartPointOffset.Dy = -21;
                belka.EndPointOffset.Dy = -21;
            }

            return belka;
        }

        private static BoltArray sruby(Point x, Point y, Part dop)

        {
            BoltArray bg = new BoltArray();

            bg.FirstPosition = x;
            bg.SecondPosition = y;
            bg.Bolt = true;
            bg.Nut1 = true;
            bg.Washer1 = true;
            bg.Washer2 = false;
            bg.Washer3 = true;
            bg.Position.Depth = Position.DepthEnum.MIDDLE;
            bg.Position.DepthOffset = 0;
            bg.Position.Plane = Position.PlaneEnum.MIDDLE;
            bg.Position.PlaneOffset = 0;
            bg.Position.Rotation = Position.RotationEnum.TOP;
            bg.Position.RotationOffset = 0;
            bg.BoltType = BoltArray.BoltTypeEnum.BOLT_TYPE_SITE;
            bg.PartToBeBolted = dop;
            bg.PartToBoltTo = dop;
            bg.AddBoltDistX(0);
            bg.AddBoltDistY(0);
            bg.CutLength = 50;
            bg.StartPointOffset.Dx = 30;


            return bg;

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        public void textBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
           
        }
        private static Point dlugosc(Point x, Point y, double l)
        {
            double x1 = x.X;
            double y1 = x.Y;
            double z1 = x.Z;
            double x2 = y.X;
            double y2 = y.Y;
            double z2 = y.Z;
            double dl = l;

            Vector v = new Vector((x1-x2)/10000, (y1-y2)/10000, (z1-z2)/10000);
            double d = v.GetLength();
            double skrocenie = dl - 180;
            double ile = skrocenie / d;
            Vector v2 = ile * v;
            Point p4 = new Point(y);
            p4.Translate(v2.X, v2.Y, v2.Z);

            return p4;

        }

        

        public void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
           
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}