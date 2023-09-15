namespace Tweens.Core {
  public class Nullable<Type> {
    internal Type Value { get; private set; }
    internal bool HasValue { get; private set; }

    internal Nullable() { }

    internal Nullable(Type value) {
      Value = value;
      HasValue = true;
    }

    internal Nullable(Nullable<Type> nullable) {
      Value = nullable.Value;
      HasValue = nullable.HasValue;
    }

    internal void Unset() {
      Value = default;
      HasValue = false;
    }

    public static implicit operator Type(Nullable<Type> test) {
      return test.Value;
    }

    public static implicit operator Nullable<Type>(Type value) {
      return new Nullable<Type> { Value = value, HasValue = true };
    }
  }
}