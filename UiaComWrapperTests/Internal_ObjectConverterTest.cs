using System.Drawing;
using System.Windows.Automation;
using Interop.UIAutomationClient;
using NUnit.Framework;
using UIAComWrapperInternal;

namespace UIAComWrapperTests
{
    public struct ObjectTestMapping
    {
        public AutomationProperty property;
        public object input;
        public object expected;

        public ObjectTestMapping(AutomationProperty property, object input, object expected)
        {
            this.property = property;
            this.input = input;
            this.expected = expected;
        }
    };

    /// <summary>
    /// Tests for the ObjectConverter system that 'polishes' returned objects
    /// into the right types for this API.
    /// </summary>
    [TestFixture]
    public class ObjectConverterTest
    {
        [Test]
        public void TestObjectConverter()
        {
            ObjectTestMapping[] testMap = new ObjectTestMapping[] {
                new ObjectTestMapping(AutomationElement.BoundingRectangleProperty, new double[] {1, 2, 3, 4}, new Rectangle(1, 2, 3, 4)), 
                new ObjectTestMapping(AutomationElement.ControlTypeProperty, (int)50000, ControlType.Button), 
                new ObjectTestMapping(AutomationElement.ClickablePointProperty, new double[] {1, 2}, new Point(1, 2)), 
                new ObjectTestMapping(AutomationElement.CultureProperty, 0x0409, new System.Globalization.CultureInfo(0x0409)), 
                new ObjectTestMapping(AutomationElement.CultureProperty, 0, System.Globalization.CultureInfo.InvariantCulture),
                new ObjectTestMapping(AutomationElement.LabeledByProperty, null, null), 
                new ObjectTestMapping(AutomationElement.LabeledByProperty, Automation.Factory.GetRootElement(), AutomationElement.RootElement), 
                new ObjectTestMapping(AutomationElement.OrientationProperty, 2, OrientationType.OrientationType_Vertical),
                new ObjectTestMapping(DockPattern.DockPositionProperty, 4, DockPosition.DockPosition_Fill), 
                new ObjectTestMapping(ExpandCollapsePattern.ExpandCollapseStateProperty, 2, ExpandCollapseState.ExpandCollapseState_PartiallyExpanded),
                new ObjectTestMapping(WindowPattern.WindowVisualStateProperty, 2, WindowVisualState.WindowVisualState_Minimized),
                new ObjectTestMapping(WindowPattern.WindowInteractionStateProperty, 1, WindowInteractionState.WindowInteractionState_Closing),
                new ObjectTestMapping(TablePattern.RowOrColumnMajorProperty, 2, RowOrColumnMajor.RowOrColumnMajor_Indeterminate),
                new ObjectTestMapping(TogglePattern.ToggleStateProperty, 1, ToggleState.ToggleState_On)
            };

            foreach (ObjectTestMapping mapping in testMap)
            {
                PropertyTypeInfo info;
                Schema.GetPropertyTypeInfo(mapping.property, out info);
                object output = mapping.input;
                if (info != null && info.ObjectConverter != null)
                {
                    output = info.ObjectConverter(mapping.input);
               } 
                else
                {
                    output = Utility.WrapObjectAsProperty(mapping.property, mapping.input);
                }
                Assert.IsTrue(output == null || info == null || output.GetType() == info.Type);
                Assert.AreEqual(output, mapping.expected);
            }
        }
    }
}
