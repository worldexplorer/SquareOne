using System;

using Sq1.Core;

namespace Sq1.Widgets.RangeBar {
	public abstract partial class RangeBar<T>  {
		public event EventHandler<RangeArgs<T>> OnValueMinChanged;
		public event EventHandler<RangeArgs<T>> OnValueMaxChanged;
		public event EventHandler<RangeArgs<T>> OnValuesMinAndMaxChanged;
		public event EventHandler<RangeArgs<T>> OnValueMouseOverChanged;

		protected virtual void RaiseOnValueMinChanged() {
			if (this.OnValueMinChanged == null) return;
			RangeArgs<T> args = this.createEventArgsSnapshot();
			this.OnValueMinChanged(this, args);
		}
		protected virtual void RaiseOnValueMaxChanged() {
			if (this.OnValueMaxChanged == null) return;
			RangeArgs<T> args = this.createEventArgsSnapshot();
			this.OnValueMaxChanged(this, args);
		}
		protected virtual void RaiseOnValuesMinAndMaxChanged() {
			if (this.OnValuesMinAndMaxChanged == null) return;
			RangeArgs<T> args = this.createEventArgsSnapshot();
			this.OnValuesMinAndMaxChanged(this, args);
		}
		protected virtual void RaiseOnValueMouseOverChanged() {
			if (this.OnValueMouseOverChanged == null) return;
			RangeArgs<T> args = this.createEventArgsSnapshot();
			this.OnValueMouseOverChanged(this, args);
		}
		RangeArgs<T> createEventArgsSnapshot() {
			return new RangeArgs<T>(this.RangeMin, this.RangeMinFormatted,
							 		this.RangeMax, this.RangeMaxFormatted,
							 		this.ValueMin, this.ValueMinFormatted,
							 		this.ValueMax, this.ValueMaxFormatted,
									this.ValueMouseOverFromRangePercentage, this.ValueMouseOverFormatted);
		}
	}
}