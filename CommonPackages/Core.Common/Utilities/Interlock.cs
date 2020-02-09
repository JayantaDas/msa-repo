namespace Core.Common
{
    public class Interlock<T>
    {
        public T Value { get; private set; }

        public static implicit operator T(Interlock<T> value)
        {
            return value.Value;
        }

        public static implicit operator Interlock<T>(T value)
        {
            return new Interlock<T>() { Value = value };
        }

        private Interlock()
        {
        }

        public override string ToString()
        {
            return this.Value.ToString();
        }
    }
}
