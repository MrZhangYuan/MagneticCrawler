using System;
using MagneticCrawler.Crawlers;
using UltimateCore;

namespace MagneticCrawler
{
        public class Filter
        {
                public event Action<object> ConditionChanged;

                private ItemType _itemType = ItemType.All;
                public ItemType ItemType
                {
                        get
                        {
                                return _itemType;
                        }
                        set
                        {
                                if (this._itemType != value)
                                {
                                        _itemType = value;
                                        this._timerBuffer.ReSet(this);
                                }
                        }
                }

                private int? _sizeStart;
                public int? SizeStart
                {
                        get
                        {
                                return _sizeStart;
                        }
                        set
                        {
                                if (_sizeStart != value)
                                {
                                        _sizeStart = value;
                                        this._timerBuffer.ReSet(this);
                                }
                        }
                }


                private int? _sizeEnd;
                public int? SizeEnd
                {
                        get
                        {
                                return _sizeEnd;
                        }
                        set
                        {
                                if (_sizeEnd != value)
                                {
                                        _sizeEnd = value;
                                        this._timerBuffer.ReSet(this);
                                }
                        }
                }


                private DateTime? _dateStart;
                public DateTime? DateStart
                {
                        get
                        {
                                return _dateStart;
                        }
                        set
                        {
                                if (_dateStart != value)
                                {
                                        _dateStart = value;
                                        this._timerBuffer.ReSet(this);
                                }
                        }
                }


                private DateTime? _dateEnd;
                public DateTime? DateEnd
                {
                        get
                        {
                                return _dateEnd;
                        }
                        set
                        {
                                if (_dateEnd != value)
                                {
                                        _dateEnd = value;
                                        this._timerBuffer.ReSet(this);
                                }
                        }
                }


                private string _text;
                public string Text
                {
                        get
                        {
                                return _text;
                        }
                        set
                        {
                                if (_text != value)
                                {
                                        _text = value;
                                        this._timerBuffer.ReSet(this);
                                }
                        }
                }

                private readonly TimerBuffer<object> _timerBuffer;
                public Filter()
                {
                        _timerBuffer = new TimerBuffer<object>
                        {
                                DueTime = 100,
                                Action = _p =>
                                {
                                        this.OnConditionChanged(_p);
                                }
                        };
                }

                protected void OnConditionChanged(object param)
                {
                        this.ConditionChanged?.Invoke(param);
                }
        }
}
