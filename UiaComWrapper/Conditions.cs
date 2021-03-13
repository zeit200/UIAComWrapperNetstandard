// (c) Copyright Microsoft, 2012.
// This source is subject to the Microsoft Permissive License.
// See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
// All other rights reserved.



using System;
using System.Globalization;
using System.Windows;
using System.Diagnostics;
using UIAComWrapperInternal;
using Interop.UIAutomationClient;

namespace System.Windows.Automation
{
    public abstract class Condition
    {
        
        public static readonly Condition FalseCondition = BoolCondition.Wrap(false);
        public static readonly Condition TrueCondition = BoolCondition.Wrap(true);

        internal abstract IUIAutomationCondition NativeCondition { get; }

        internal static Condition Wrap(IUIAutomationCondition obj)
        {
            if (obj is IUIAutomationBoolCondition)
                return new BoolCondition((IUIAutomationBoolCondition)obj);
            else if (obj is IUIAutomationAndCondition)
                return new AndCondition((IUIAutomationAndCondition)obj);
            else if (obj is IUIAutomationOrCondition)
                return new OrCondition((IUIAutomationOrCondition)obj);
            else if (obj is IUIAutomationNotCondition)
                return new NotCondition((IUIAutomationNotCondition)obj);
            else if (obj is IUIAutomationPropertyCondition)
                return new PropertyCondition((IUIAutomationPropertyCondition)obj);
            else
                throw new ArgumentException("obj");
        }

        internal static IUIAutomationCondition ConditionManagedToNative(
            Condition condition)
        {
            return (condition == null) ? null : condition.NativeCondition;
        }

        internal static IUIAutomationCondition[] ConditionArrayManagedToNative(
            Condition[] conditions)
        {
            IUIAutomationCondition[] unwrappedConditions =
                new IUIAutomationCondition[conditions.Length];
            for (int i = 0; i < conditions.Length; ++i)
            {
                unwrappedConditions[i] = ConditionManagedToNative(conditions[i]);
            }
            return unwrappedConditions;
        }

        internal static Condition[] ConditionArrayNativeToManaged(
            Array conditions)
        {
            Condition[] wrappedConditions = new Condition[conditions.Length];
            for (int i = 0; i < conditions.Length; ++i)
            {
                wrappedConditions[i] = Wrap((IUIAutomationCondition)conditions.GetValue(i));
            }
            return wrappedConditions;
        }

        
        private class BoolCondition : Condition
        {
            
            internal IUIAutomationBoolCondition _obj;

            internal BoolCondition(IUIAutomationBoolCondition obj)
            {
                Debug.Assert(obj != null);
                this._obj = obj;
            }

            internal override IUIAutomationCondition NativeCondition
            {
                get { return this._obj; }
            }

            internal static BoolCondition Wrap(bool b)
            {
                IUIAutomationBoolCondition obj = (IUIAutomationBoolCondition)((b) ?
                    Automation.Factory.CreateTrueCondition() :
                    Automation.Factory.CreateFalseCondition());
                return new BoolCondition(obj);
            }
        }
    }

    public class NotCondition : Condition
    {
        
        internal IUIAutomationNotCondition _obj;

        
        internal NotCondition(IUIAutomationNotCondition obj)
        {
            Debug.Assert(obj != null);
            this._obj = obj;
        }

        public NotCondition(Condition condition)
        {
            this._obj = (IUIAutomationNotCondition)
                Automation.Factory.CreateNotCondition(
                ConditionManagedToNative(condition));
        }

        internal override IUIAutomationCondition NativeCondition
        {
            get { return this._obj; }
        }

        
        public Condition Condition
        {
            get
            {
                return Wrap(this._obj.GetChild());
            }
        }
    }
    
    public class AndCondition : Condition
    {
        
        internal IUIAutomationAndCondition _obj;

        
        internal AndCondition(IUIAutomationAndCondition obj)
        {
            Debug.Assert(obj != null);
            this._obj = obj;
        }

        public AndCondition(params Condition[] conditions)
        {
            this._obj = (IUIAutomationAndCondition)
                Automation.Factory.CreateAndConditionFromArray(
                ConditionArrayManagedToNative(conditions));
        }

        internal override IUIAutomationCondition NativeCondition
        {
            get { return this._obj; }
        }

        public Condition[] GetConditions()
        {
            return ConditionArrayNativeToManaged(this._obj.GetChildren());
        }
    }

    public class OrCondition : Condition
    {
        
        internal IUIAutomationOrCondition _obj;

        
        internal OrCondition(IUIAutomationOrCondition obj)
        {
            Debug.Assert(obj != null);
            this._obj = obj;
        }

        public OrCondition(params Condition[] conditions)
        {
            this._obj = (IUIAutomationOrCondition)
                Automation.Factory.CreateOrConditionFromArray(
                ConditionArrayManagedToNative(conditions));
        }

        internal override IUIAutomationCondition NativeCondition
        {
            get { return this._obj; }
        }

        public Condition[] GetConditions()
        {
            return ConditionArrayNativeToManaged(this._obj.GetChildren());
        }
    }

    public class PropertyCondition : Condition
    {
        
        internal IUIAutomationPropertyCondition _obj;

        
        internal PropertyCondition(IUIAutomationPropertyCondition obj)
        {
            Debug.Assert(obj != null);
            this._obj = obj;
        }

        public PropertyCondition(AutomationProperty property, object value)
        {
            this.Init(property, value, PropertyConditionFlags.PropertyConditionFlags_None);
        }

        public PropertyCondition(AutomationProperty property, object value, PropertyConditionFlags flags)
        {
            this.Init(property, value, flags);
        }

        private void Init(AutomationProperty property, object val, PropertyConditionFlags flags)
        {
            Utility.ValidateArgumentNonNull(property, "property");

            this._obj = (IUIAutomationPropertyCondition)
                Automation.Factory.CreatePropertyConditionEx(
                property.Id,
                Utility.UnwrapObject(val), 
                (PropertyConditionFlags)flags);
        }

        internal override IUIAutomationCondition NativeCondition
        {
            get { return this._obj; }
        }

        
        public PropertyConditionFlags Flags
        {
            get
            {
                return (PropertyConditionFlags)this._obj.PropertyConditionFlags;
            }
        }

        public AutomationProperty Property
        {
            get
            {
                return AutomationProperty.LookupById(this._obj.propertyId);
            }
        }

        public object Value
        {
            get
            {
                return this._obj.PropertyValue;
            }
        }
    }
}
