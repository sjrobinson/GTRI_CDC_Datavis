﻿using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Text;
using Microsoft.Surface;
using Microsoft.Surface.Presentation;
using Microsoft.Surface.Presentation.Controls;
using Microsoft.Surface.Presentation.Input;
using System.Globalization;
using TangibleAnchoring.Submissions;
using TangibleAnchoring.Config;

namespace TangibleAnchoring
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : SurfaceWindow
    {
        private const string configFile = "Config.xml";
        private readonly Config.Config configData;
        private readonly Submissions.SubmissionData submissionData;
        private Ellipse[] dataPointEllipses;
        Ellipse haloEllipse = new Ellipse();

        //WA = Work Area

        private double WAWidth = System.Windows.SystemParameters.WorkArea.Width;
        private double WAHeight = System.Windows.SystemParameters.WorkArea.Height;
        double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
        double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
        private double leftMargin = 100;
        private double bottomMargin = 50;
        private double rightMargin = 50;
        private double topMargin = 50;
        private double YAxisLength;
        private double XAxisLength;
        

        // The following distances specify how far the description textbox should be
        // from the center of the touch device.
        private const double nonTagDescriptionXDistance = 40.0;
        private const double tagDescriptionXDistance = 190.0;
        private const double descriptionYDistance = 30.0;

        public MainWindow()
        {
            InitializeComponent();
            // Add handlers for window availability events
            AddWindowAvailabilityHandlers();

            configData = new Config.Config(configFile);
            // Plug our item data into our visualizations
            submissionData = new Submissions.SubmissionData(configData.SubmissionsFileUri);
            dataPointEllipses = new Ellipse[submissionData.Submissions.Length];

            LogMsg(configData.Questions.Length.ToString());

            //setQuestion("49");

            // Initialize the XAxis
            XAxisLabel.Content = configData.FindQuestionFromId("46").QuestionText;
            XAxisLabel.Uid = "46";
            XAxis.Uid = "46";

            // Initialize the YAxis
            YAxisLabel.Content = configData.FindQuestionFromId("4").QuestionText;
            YAxisLabel.Uid = "4";
            YAxis.Uid = "4";

            YAxisLength = YAxis.Y2 - YAxis.Y1;
            XAxisLength = XAxis.X2 - XAxis.X1;

            DrawPoints(submissionData);
            DrawTicks("XAxis");
            LogMsg(configData.FindTangibleFromId("8").Rotation[0].FacetRange);
            //LogMsg(CurrentQuestion.Uid);
            //Criteria criteria = new Criteria("49","1,2");
            //VizOperationFilter(criteria);

            
        }

        /// <summary>
        /// Shows a log message on the top left.
        /// </summary>
        /// <param name="message">string message</param>
        private void LogMsg(string message)
        {
            LogMessageLabel.Content = message;
        }

        /// <summary>
        /// Logic for drawing X, Y axes.
        /// </summary>
        //private void DrawAxes()
        //{

        //    string message = "WA: W= " + screenWidth + " H= " + screenHeight;
        //    LogMsg(message);
        //    Line xAxis = new Line(), yAxis = new Line();
        //    //xAxis.Name = "XAxis";
        //    //xAxis.Stroke = System.Windows.Media.Brushes.Beige;
        //    //xAxis.X1 = leftMargin / 2;
        //    //xAxis.Y1 = WAHeight - bottomMargin / 2;
        //    //xAxis.X2 = WAWidth - rightMargin;
        //    //xAxis.Y2 = xAxis.Y1;
        //    ////xAxis.HorizontalAlignment = HorizontalAlignment.Left;
        //    ////xAxis.VerticalAlignment = VerticalAlignment.Center;
        //    //xAxis.StrokeThickness = 1;
        //    //MainCanvas.Children.Add(xAxis);

        //    //yAxis.Stroke = System.Windows.Media.Brushes.Beige;
        //    //xAxis.Name = "YAxis";
        //    //yAxis.X1 = leftMargin / 2;
        //    //yAxis.Y1 = WAHeight - bottomMargin / 2;
        //    //yAxis.X2 = yAxis.X1;
        //    //yAxis.Y2 = bottomMargin + topMargin;
        //    ////yAxis.HorizontalAlignment = HorizontalAlignment.Left;
        //    ////yAxis.VerticalAlignment = VerticalAlignment.Center;
        //    //yAxis.StrokeThickness = 1;
        //    //MainCanvas.Children.Add(yAxis);

        //}

        private void DrawTicks(string axis)
        {
            string qId; 
            if (axis == "XAxis")
            {
                qId = XAxis.Uid;
                Question ques = configData.FindQuestionFromId(qId);
                //int numAnswers = ques.Answers.Length;
                //double step = XAxisLength / numAnswers;
                //for (int index = 0; index < numAnswers; index++)
                //{
                //    Line tick = new Line();
                //    tick.Name = "XTick_"+ index;
                //    tick.Stroke = System.Windows.Media.Brushes.DarkSlateGray;
                //    tick.X1 = XAxis.X1 + index*step;
                //    tick.Y1 = XAxis.Y1 - 10.00;
                //    tick.X2 = XAxis.X1 + index * step;
                //    tick.Y2 = XAxis.Y1 + 10.00;
                //    //xAxis.HorizontalAlignment = HorizontalAlignment.Left;
                //    //xAxis.VerticalAlignment = VerticalAlignment.Center;
                //    tick.StrokeThickness = 1;
                //    MainCanvas.Children.Add(tick);
                //}
            } 
            else if (axis == "YAxis")
            {
                qId = YAxis.Uid;
                Question ques = configData.FindQuestionFromId(qId);
                
            }

        }

        /// <summary>
        /// Logic for drawing X, Y axes labels.
        /// </summary>
        private void DrawAxesLabels(string xAxisLabel, string yAxisLabel)
        { 
            
        }

        /// <summary>
        /// Logic for drawing data points.
        /// </summary>
        /// <param name="submissionData">collection of points</param>
        private void DrawPoints(Submissions.SubmissionData submissionData)
        {
            if (submissionData != null)
            {
                int numPoints = submissionData.Submissions.Length;


                SolidColorBrush[] viewpointColors = {
                                           SurfaceColors.Accent1Brush,
                                           SurfaceColors.Accent2Brush, 
                                           SurfaceColors.Accent3Brush, 
                                           SurfaceColors.Accent4Brush,
                                           SurfaceColors.BulletBrush, 
                                           SurfaceColors.ButtonBackgroundBrush, 
                                           SurfaceColors.ButtonBackgroundPressedBrush
                                       };

                for (int index = 0; index < numPoints; index++)
                {
                    dataPointEllipses[index] = new Ellipse();
                    Submissions.Submission sData = submissionData.Submissions[index];
                    //TODO replace 700 by minimum value for the variable
                    double leftPosition = (double)(((int.Parse(sData.UserId) - 700) * 2) % WAWidth);
                    double bottomPosition = (double)(((sData.Age) * 10) % WAHeight);
                    dataPointEllipses[index].SetValue(Canvas.LeftProperty, leftPosition);
                    dataPointEllipses[index].SetValue(Canvas.BottomProperty, bottomPosition);
                    dataPointEllipses[index].Name = "data_" + index;
                    dataPointEllipses[index].Uid += index; 
                    
                    //Dynamic assignment of touch event handler
                    dataPointEllipses[index].TouchEnter += new EventHandler<TouchEventArgs>(DataPointTouchEnter);
                    dataPointEllipses[index].TouchLeave += new EventHandler<TouchEventArgs>(DataPointTouchLeave);
                    dataPointEllipses[index].Height = 20;
                    dataPointEllipses[index].Width = 20;

                    // AnswerId - 1 because viewpoints are numbered from 1..7 
                    dataPointEllipses[index].Fill = viewpointColors[int.Parse(sData.Responses[0].AnswerId) - 1];
                    MainCanvas.Children.Add(dataPointEllipses[index]);
                }
            }
        }

        private void setQuestion(string questionId)
        {
            //get the question questionText
            string qText = configData.FindQuestionFromId(questionId).QuestionText;
            CurrentQuestion.Content = qText;
            CurrentQuestion.Uid = questionId;
           // LogMsg(qText);
        }
         /// <summary>
        /// Touch Event handler for dynamically added data point.
        /// </summary>
        /// <param name="sender">element that invokes this handler (e.g an Ellipse)</param>
        /// <param name="e">touch event information such type of device</param>
        private void DataPointTouchLeave(object sender, TouchEventArgs e)
        {
            Ellipse senderEllipse = sender as Ellipse;
            MainCanvas.Children.Remove(haloEllipse);
            Description.Visibility = Visibility.Hidden;
            ConnectingLine.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Touch Event handler for dynamically added data point.
        /// </summary>
        /// <param name="sender">element that invokes this handler (e.g an Ellipse)</param>
        /// <param name="e">touch event information such type of device</param>
        private void DataPointTouchEnter(object sender, TouchEventArgs e)
        {
            Ellipse senderEllipse = sender as Ellipse;
            
            haloEllipse.SetValue(Canvas.LeftProperty, Canvas.GetLeft(senderEllipse) - senderEllipse.Width/4);
            haloEllipse.SetValue(Canvas.BottomProperty, Canvas.GetBottom(senderEllipse) - senderEllipse.Height/4);
            haloEllipse.Height = 30;
            haloEllipse.Width = 30;
            haloEllipse.Stroke = SurfaceColors.BulletBrush;
            haloEllipse.StrokeThickness = 2;
            MainCanvas.Children.Remove(haloEllipse); //FIX: Next line throws ArgumentException if another haloEllipse exists while new is added.
            MainCanvas.Children.Add(haloEllipse);

            int submissionIndex = int.Parse(senderEllipse.Name.Split('_')[1]);
            Submission submission = submissionData.Submissions[submissionIndex];
            if (senderEllipse != null)
            {
                LogMsg(submission.UserId);

                //Interactively remove elements that have been touched (could make for a game)
                //MainCanvas.Children.Remove(senderEllipse);
            }

            //This will produce details on demand
            UpdateDescription(RootGrid, e.TouchDevice, submission, true);

            // Capture to the ellipse.  
            //e.TouchDevice.Capture(senderEllipse);

            SolidColorBrush mySolidColorBrush = new SolidColorBrush();

            // Describes the brush's color using RGB values.  
            // Each value has a facetRange of 0-255.
            mySolidColorBrush.Color = SurfaceColors.Accent2Color;

            //Used to change color of touched elements.
            //senderEllipse.Fill = mySolidColorBrush;


            // Mark this event as handled.  
            e.Handled = true;
        }

        /// <summary>
        /// Gets a string that describes the type of touch device on the surface
        /// </summary>
        /// <param name="touchDevice">the touch device to be examined</param>
        /// <returns>a string that describes the type of touch device</returns>
        private static string GetTouchDeviceTypeString(TouchDevice touchDevice)
        {
            if (touchDevice.GetTagData() != TagData.None)
            {
                return "Tag";
            }
            if (touchDevice.GetIsFingerRecognized())
            {
                return "Finger";
            }

            return "Blob";
        }

        /// <summary>
        /// Update the questionText description with the most recent property values. Position
        /// the textbox so that it does not go offscreen (outside parentGrid). Also
        /// position the connecting line between the touch device and the textbox.
        /// </summary>
        /// <param name="parentGrid">the container for this diagram-
        /// description questionText will not go outside of this container's bounds</param>
        /// <param name="touchDevice">the touch device to diagram</param>
        /// <param name="showTouchDeviceInfo">Whether or not the touch device info will be visible</param>
        private void UpdateDescription(Grid parentGrid, TouchDevice touchDevice, Submission submission, bool showTouchDeviceInfo)
        {
            // Show or hide the touchDevice info based on showTouchDeviceInfo
            Description.Visibility = showTouchDeviceInfo ? Visibility.Visible : Visibility.Hidden;
            ConnectingLine.Visibility = showTouchDeviceInfo ? Visibility.Visible : Visibility.Hidden;

            if (!showTouchDeviceInfo)
            {
                // Don't need to do the calculations if info isn't going to be shown
                return;
            }

            Point position = touchDevice.GetPosition(parentGrid);
            Rect bounds = new Rect(0, 0, parentGrid.ActualWidth, parentGrid.ActualHeight);
            // Determine where around the touchDevice the description should be displayed.
            // The default position is above and to the left.
            bool isAbove = true;
            bool isLeft = true;

            // Description questionText for tags is different than non-tags
            double descriptionXDistance;
            bool isTag = touchDevice.GetIsTagRecognized();

            if (isTag)
            {
                descriptionXDistance = tagDescriptionXDistance;
            }
            else
            {
                descriptionXDistance = nonTagDescriptionXDistance;
            }

            // Put description below touchDevice if default location is out of bounds.
            Rect upperLeftBounds = GetDescriptionBounds(position, isAbove, isLeft, descriptionXDistance, descriptionYDistance);
            if (upperLeftBounds.Top < bounds.Top)
            {
                isAbove = false;
            }

            // Put description to the right of the touchDevice if default location is out of bounds.
            if (upperLeftBounds.Left < bounds.Left)
            {
                isLeft = false;
            }

            // Calculate the final bounds that will be used for the textbox position
            // based on the updated isAbove and isLeft values.
            Rect finalBounds = GetDescriptionBounds(position, isAbove, isLeft, descriptionXDistance, descriptionYDistance);
            Canvas.SetLeft(Description, finalBounds.Left);
            Canvas.SetTop(Description, finalBounds.Top);

            // Set the justification of the type in the textbox based
            // on which side of the touchDevice the textbox is on.
            if (isLeft)
            {
                Description.TextAlignment = TextAlignment.Right;
            }
            else
            {
                Description.TextAlignment = TextAlignment.Left;
            }

            // Create the description string.
            StringBuilder descriptionText = new StringBuilder();
            //Show demographic info for submission data
            descriptionText.AppendLine(String.Format(CultureInfo.InvariantCulture, "Respondent questionId: {0}", submission.UserId));
            descriptionText.AppendLine(String.Format(CultureInfo.InvariantCulture, "Age: {0}", submission.Age.ToString()));
            descriptionText.AppendLine(String.Format(CultureInfo.InvariantCulture, "Location: {0}, {1}", submission.Latitude.ToString(), submission.Longitude.ToString()));


            //descriptionText.AppendLine(String.Format(CultureInfo.InvariantCulture, "RecognizedTypes: {0}", GetTouchDeviceTypeString(touchDevice)));
            //descriptionText.AppendLine(String.Format(CultureInfo.InvariantCulture, "QuestionId: {0}", touchDevice.QuestionId));

            //// Use the "f1" format specifier to limit the amount of decimal positions shown.
            //descriptionText.AppendLine(String.Format(CultureInfo.InvariantCulture, "X: {0}", position.X.ToString("f1", CultureInfo.InvariantCulture)));
            //descriptionText.AppendLine(String.Format(CultureInfo.InvariantCulture, "Y: {0}", position.Y.ToString("f1", CultureInfo.InvariantCulture)));

            // Display "null" for Orientation if the touchDevice does not have an orientation value.
            //string orientationString;
            //double? touchDeviceOrientation = touchDevice.GetOrientation(parentGrid);
            //if (touchDeviceOrientation == null)
            //{
            //    orientationString = "null";
            //}
            //else
            //{
            //    orientationString = ((double)touchDeviceOrientation).ToString("f1", CultureInfo.InvariantCulture);
            //}
            //descriptionText.AppendLine(String.Format(CultureInfo.InvariantCulture, "Orientation: {0}", orientationString));

            //if (touchDevice.GetTagData() != TagData.None)
            //{
            //    //descriptionText.AppendLine("Schema: 0x" + touchDevice.GetTagData().Schema.ToString("x8", CultureInfo.InvariantCulture));
            //    //descriptionText.AppendLine("Series:  0x" + touchDevice.GetTagData().Series.ToString("x16", CultureInfo.InvariantCulture));
            //    //descriptionText.AppendLine("ExtendedValue: 0x" + touchDevice.GetTagData().ExtendedValue.ToString("x16", CultureInfo.InvariantCulture));
            //    descriptionText.AppendLine("Value:  0x" + touchDevice.GetTagData().Value.ToString("x4", CultureInfo.InvariantCulture));
            //}

            // Update the description textbox.
            Description.Text = descriptionText.ToString();

            // Update the line that connects the touchDevice to the description textbox.
            double x2;
            if (isLeft)
            {
                x2 = finalBounds.Right;
            }
            else
            {
                x2 = finalBounds.Left;
            }
            // Position (X1,Y1) is the center of the touchDevice.
            // Position (X2,Y2) is the edge of the description questionText box.
            ConnectingLine.X1 = position.X;
            ConnectingLine.Y1 = position.Y;
            ConnectingLine.X2 = x2;
            ConnectingLine.Y2 = finalBounds.Top + finalBounds.Height * 0.5;
        }


        /// <summary>
        /// Calculate the bounds of the textbox for the specified location and quadrant.
        /// </summary>
        /// <param name="position">position of the touch device to diagram</param>
        /// <param name="isAbove">specifies if the requested bounds is above or below the touch device position</param>
        /// <param name="isLeft">specifies if the requested bounds is right or left of the touch device position</param>
        /// <param name="xDistance">the x distance from the touch evice position</param>
        /// <param name="yDistance">the y distance from the touch device position</param>
        /// <returns>the bounds at the specified location</returns>
        private Rect GetDescriptionBounds(Point position, bool isAbove, bool isLeft, double xDistance, double yDistance)
        {
            double left;
            if (isLeft)
            {
                left = position.X - xDistance - Description.Width;
            }
            else
            {
                left = position.X + xDistance;
            }
            double top;
            if (isAbove)
            {
                top = position.Y - yDistance - Description.Height;
            }
            else
            {
                top = position.Y + yDistance;
            }
            return new Rect(left, top, Description.Width, Description.Height);
        }

        /// <summary>
        /// Filter data based on criteria
        /// </summary>
        /// <param name="criteria">Compound variable that consists of questionId and answerIDs used for filtering</param>
        private void VizOperationFilter(Criteria criteria) 
        {
            bool isMatch = false;
            Submission tempSubmission;
            int numPoints = dataPointEllipses.Length;
            for (int i = 0; i < numPoints; i++)
            {
                int sIndex = int.Parse(dataPointEllipses[i].Uid);
                tempSubmission = submissionData.Submissions[sIndex];
                int numResponses = tempSubmission.Responses.Length;
                for (int j = 0; j < numResponses; j++)
                {
                    if (tempSubmission.Responses[j].QuestionId == criteria.QuestionId)
                    {
                        int numAnswers = criteria.AnswerIds.Length;
                        
                        for (int k = 0; k < numAnswers; k++)
                        {
                            if (tempSubmission.Responses[j].AnswerId == criteria.AnswerIds[k])
                            { //Found a match
                                isMatch = true;
                            }
                        }
                        if (!isMatch)
                        {
                            dataPointEllipses[i].Visibility = System.Windows.Visibility.Hidden;
                        }
                        else
                        {
                            isMatch = false; //Reset isMatch
                            break;
                        }
                        
                    }
                }
            }
        }

        private void VizOperationReset()
        {
            int numPoints = dataPointEllipses.Length;
            for (int i = 0; i < numPoints; i++)
            {
                dataPointEllipses[i].Visibility = System.Windows.Visibility.Visible;
            }
        }


        /// <summary>
        /// Adds handlers for window availability events.
        /// </summary>
        private void AddWindowAvailabilityHandlers()
        {
            // Subscribe to surface window availability events
            ApplicationServices.WindowInteractive += OnWindowInteractive;
            ApplicationServices.WindowNoninteractive += OnWindowNoninteractive;
            ApplicationServices.WindowUnavailable += OnWindowUnavailable;
        }

        /// <summary>
        /// Removes handlers for window availability events.
        /// </summary>
        private void RemoveWindowAvailabilityHandlers()
        {
            // Unsubscribe from surface window availability events
            ApplicationServices.WindowInteractive -= OnWindowInteractive;
            ApplicationServices.WindowNoninteractive -= OnWindowNoninteractive;
            ApplicationServices.WindowUnavailable -= OnWindowUnavailable;
        }

        /// <summary>
        /// This is called when the user can interact with the application's window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowInteractive(object sender, EventArgs e)
        {
            //TODO: enable audio, animations here
        }

        /// <summary>
        /// This is called when the user can see but not interact with the application's window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowNoninteractive(object sender, EventArgs e)
        {
            //TODO: Disable audio here if it is enabled

            //TODO: optionally enable animations here
        }

        /// <summary>
        /// This is called when the application's window is not visible or interactive.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowUnavailable(object sender, EventArgs e)
        {
            //TODO: disable audio, animations here
        }

        /// <summary>
        /// This is called when a tangible is put down on the table.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">Event object containing information about tagvisualization</param>
        private void OnTangibleAdded(object sender, TagVisualizerEventArgs e)
        {
            TangibleVisualization camera = (TangibleVisualization)e.TagVisualization;
            camera.CameraModel.Content = camera.Orientation;
            camera.myEllipse.Fill = SurfaceColors.Accent1Brush;
            Criteria criteria = new Criteria("49", "1,2");
            VizOperationFilter(criteria);
            //TangibleVisualization camera = (TangibleVisualization)e.TagVisualization;
            //switch (camera.VisualizedTag.Value)
            //{
            //    case 193:
            //        camera.CameraModel.Content = "Fabrikam, Inc. ABC-12";
            //        camera.myEllipse.Fill = SurfaceColors.Accent1Brush;
            //        break;
            //    case 2:
            //        camera.CameraModel.Content = "Fabrikam, Inc. DEF-34";
            //        camera.myEllipse.Fill = SurfaceColors.Accent2Brush;
            //        break;
            //    case 3:
            //        camera.CameraModel.Content = "Fabrikam, Inc. GHI-56";
            //        camera.myEllipse.Fill = SurfaceColors.Accent3Brush;
            //        break;
            //    case 4:
            //        camera.CameraModel.Content = "Fabrikam, Inc. JKL-78";
            //        camera.myEllipse.Fill = SurfaceColors.Accent4Brush;
            //        break;
            //    default:
            //        camera.CameraModel.Content = "UNKNOWN MODEL";
            //        camera.myEllipse.Fill = SurfaceColors.ControlAccentBrush;
            //        break;
            //}
        }

        /// <summary>
        /// This is called when a tangible is moved (rotation, translation) on the table.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">Event object containing information about tagvisualization</param>
        private void OnTangibleMoved(object sender, TagVisualizerEventArgs e)
        {
            TangibleVisualization camera = (TangibleVisualization)e.TagVisualization;
            double orientation = camera.Orientation;
            camera.CameraModel.Content = orientation;
            Criteria criteria;
            VizOperationReset();

            //TODO Code below is a POC of how a rotation tangible would work. Modify to make it react differently to different types of tangibles.
            if (orientation > 0.0 && orientation <= 120.00)
            {
                criteria = new Criteria("49", "1,2");
                VizOperationFilter(criteria);
            }
            else if (orientation > 120.0 && orientation <= 240.00)
            {
                criteria = new Criteria("49", "3,4");
                VizOperationFilter(criteria);
            }
            else if (orientation > 240.0 && orientation < 360.00)
            {
                criteria = new Criteria("49", "5,6");
                VizOperationFilter(criteria);
            }

        }

        /// <summary>
        /// This is called when a tangible is removed from the table. 
        /// Note: This event is called only after the "LostTagTimeout" has completed, so expect delays, 
        /// but be thankful otherwise things will start flickering due to instability of detection of a marker.   
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">Event object containing information about tagvisualization</param>
        private void OnTangibleRemoved(object sender, TagVisualizerEventArgs e)
        {
            //TODO: Replace next line with a better function that only undoes the effect of the tangible that was just removed and not resets everything.
            VizOperationReset();
        }
    }
}
