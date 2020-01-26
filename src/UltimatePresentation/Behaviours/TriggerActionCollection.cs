using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace UltimatePresentation.Behaviours
{
        public class TriggerActionCollection : AttachableCollection<TriggerAction>
        {
                internal TriggerActionCollection()
                {
                }
                protected override void OnAttached()
                {
                        foreach (TriggerAction current in this)
                        {
                                current.Attach(base.AssociatedObject);
                        }
                }
                protected override void OnDetaching()
                {
                        foreach (TriggerAction current in this)
                        {
                                current.Detach();
                        }
                }
                internal override void ItemAdded(TriggerAction item)
                {
                        if (item.IsHosted)
                        {
                                throw new InvalidOperationException("CannotHostTriggerActionMultipleTimesExceptionMessage");
                        }
                        if (base.AssociatedObject != null)
                        {
                                item.Attach(base.AssociatedObject);
                        }
                        item.IsHosted = true;
                }
                internal override void ItemRemoved(TriggerAction item)
                {
                        if (((IAttachedObject)item).AssociatedObject != null)
                        {
                                item.Detach();
                        }
                        item.IsHosted = false;
                }
                protected override Freezable CreateInstanceCore()
                {
                        return new TriggerActionCollection();
                }
        }

}
