using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.ProductInterface.PowerSHAPE;
using Autodesk.Geometry;

namespace PowershapeLab1
{
    class Program
    {
        static double length = 200.0;
        static double width = length * ((double)(80.0) / 130.0);

        static double distanceToCentralCenter = length * ((double)(50.0) / 130.0);

        static PSModel psModel;

        static void Main(string[] args)
        {
            PSAutomation powerSHAPE = new PSAutomation(Autodesk.ProductInterface.InstanceReuse.CreateNewInstance);
            powerSHAPE.Reset();

            //Creating and selecting model
            psModel = powerSHAPE.Models.CreateEmptyModel();

            //Turning off messages
            powerSHAPE.FormUpdateOff();
            powerSHAPE.RefreshOff();
            powerSHAPE.DialogsOff();

            // Building model
            BuildingBasis();
            BuildingCylinder();
            CuttingCirclesInBase();

            // Ending execution
            //SaveModel();
            ClosingSafely(powerSHAPE);
        }
        
        static void BuildingBasis()
        {
            double halfLen = length / 2.0;
            double basisHeight = length * ((double)(12.0) / 130.0);
            double marginFromCenterToHillBorder = length * ((double)(21.0) / 130.0);
            double upperHillHeight = length * ((double)(19.0) / 130.0);

            // Points
            Point origin = new Point(0.0, 0.0, 0.0);
            Point leftBottomEndPoint = new Point(-halfLen, 0.0, 0.0);
            Point rightBottomEndPoint = new Point(halfLen, 0.0, 0.0);

            Point leftUpperEndPoint = new Point(-halfLen, basisHeight, 0.0);
            Point rightUpperEndPoint = new Point(halfLen, basisHeight, 0.0);

            Point leftBottomHillPoint = new Point(-marginFromCenterToHillBorder, basisHeight, 0.0);
            Point rightBottomHillPoint = new Point(marginFromCenterToHillBorder, basisHeight, 0.0);

            Point leftUpperHillPoint = new Point(-marginFromCenterToHillBorder, upperHillHeight, 0.0);
            Point rightUpperHillPoint = new Point(marginFromCenterToHillBorder, upperHillHeight, 0.0);

            // Lines
            PSLine leftBottomLine = psModel.Lines.CreateLine(origin, leftBottomEndPoint);
            PSLine rightBottomLine = psModel.Lines.CreateLine(origin, rightBottomEndPoint);

            PSLine leftBorderLine = psModel.Lines.CreateLine(leftBottomEndPoint, leftUpperEndPoint);
            PSLine rightBorderLine = psModel.Lines.CreateLine(rightBottomEndPoint, rightUpperEndPoint);

            PSLine leftUpperBasisLine = psModel.Lines.CreateLine(leftUpperEndPoint, leftBottomHillPoint);
            PSLine rightUpperBasisLine = psModel.Lines.CreateLine(rightUpperEndPoint, rightBottomHillPoint);

            PSLine leftHillLine = psModel.Lines.CreateLine(leftBottomHillPoint, leftUpperHillPoint);
            PSLine rightHillLine = psModel.Lines.CreateLine(rightBottomHillPoint, rightUpperHillPoint);
            PSLine upperHillLine = psModel.Lines.CreateLine(leftUpperHillPoint, rightUpperHillPoint);

            // Building wireframe
            List<PSWireframe> lines = new List<PSWireframe>()
            {
                leftBottomLine,
                rightBottomLine,
                leftBorderLine,
                rightBorderLine,
                leftUpperBasisLine,
                rightUpperBasisLine,
                leftHillLine,
                rightHillLine,
                upperHillLine
            };

            // Extruding basis
            psModel.Solids.CreateSolidExtrusionsFromWireframes(lines, width, 0.0);
        }

        static void BuildingCylinder()
        {
            double radius = length * ((double)(38.0) / 130.0);

            // Points
            Point circleCenter = new Point(0.0, distanceToCentralCenter, 0.0);
            Point startPoint = new Point(radius, distanceToCentralCenter, 0.0);

            // Circle
            PSArc circle = psModel.Arcs.CreateArcCircle(circleCenter, startPoint, radius);

            // Extrusion
            psModel.Solids.CreateSolidExtrusionFromWireframe(circle, width, 0.0);
        }

        static void CuttingCirclesInBase()
        {
            double lengthToCircleCenter = length * ((double)(50.0) / 130.0);
            double widthToBottomCircleCenter = length * ((double)(17.0) / 130.0);
            double widthToUpperCircleCenter = length * ((double)(63.0) / 130.0);
            double radius = length * ((double)(7.0) / 130.0);

            // Points
            Point leftBottomCircleCenter = new Point(-lengthToCircleCenter, 0.0, widthToBottomCircleCenter);
            Point leftUpperCircleCenter = new Point(-lengthToCircleCenter, 0.0, widthToUpperCircleCenter);
            Point rightBottomCircleCenter = new Point(lengthToCircleCenter, 0.0, widthToBottomCircleCenter);
            Point rightUpperCircleCenter = new Point(lengthToCircleCenter, 0.0, widthToUpperCircleCenter);
            
            Point leftBottomCircleStartingPoint = new Point(-lengthToCircleCenter, 0.0 + radius, widthToBottomCircleCenter);
            Point leftUpperCircleStartingPoint = new Point(-lengthToCircleCenter, 0.0 + radius, widthToUpperCircleCenter);
            Point rightBottomCircleStartingPoint = new Point(lengthToCircleCenter, 0.0 + radius, widthToBottomCircleCenter);
            Point rightUpperCircleStartingPoint = new Point(lengthToCircleCenter, 0.0 + radius, widthToUpperCircleCenter);

            // Circles
            PSArc leftBottomCircle = psModel.Arcs.CreateArcCircle(leftBottomCircleCenter, leftBottomCircleStartingPoint, radius);
            PSArc leftUpperCircle = psModel.Arcs.CreateArcCircle(leftUpperCircleCenter, leftUpperCircleStartingPoint, radius);
            PSArc rightBottomCircle = psModel.Arcs.CreateArcCircle(rightBottomCircleCenter, rightBottomCircleStartingPoint, radius);
            PSArc rightUpperCircle = psModel.Arcs.CreateArcCircle(rightUpperCircleCenter, rightUpperCircleStartingPoint, radius);

            leftBottomCircle.Rotate(Autodesk.Axes.X, 90.0, 0);
            leftUpperCircle.Rotate(Autodesk.Axes.X, 90.0, 0);
            rightBottomCircle.Rotate(Autodesk.Axes.X, 90.0, 0);
            rightUpperCircle.Rotate(Autodesk.Axes.X, 90.0, 0);
            // Cutting
            psModel.Solids.CreateSolidExtrusionFromWireframe(leftBottomCircle, (8.0 / 130.0) * width, (8.0 / 130.0) * width);
            psModel.Solids.CreateSolidExtrusionFromWireframe(leftUpperCircle, (8.0 / 130.0) * width, (8.0 / 130.0) * width);
            psModel.Solids.CreateSolidExtrusionFromWireframe(rightBottomCircle, (8.0 / 130.0) * width, (8.0 / 130.0) * width);
            psModel.Solids.CreateSolidExtrusionFromWireframe(rightUpperCircle, (8.0 / 130.0) * width, (8.0 / 130.0) * width);

            
           
            

        }

        static void SaveModel()
        {
            Autodesk.FileSystem.File exportLocation = new Autodesk.FileSystem.File(@"C:\Users\Alex\Desktop\Lab2.psmodel");
            psModel.Save(exportLocation);
        }

        static void ClosingSafely(PSAutomation powerSHAPE)
        {
            powerSHAPE.FormUpdateOn();
            powerSHAPE.RefreshOn();
            powerSHAPE.DialogsOn();

        }
    }
}
