using UnityEngine;

namespace Loxodon.Framework.Views.Variables
{
    [System.Serializable]
    public enum VariableType
    {
        Object,
        GameObject,
        Component,
        Boolean,
        Integer,
        Float,
        String,
        Color,
        Vector2,
        Vector3,
        Vector4
    }

    [System.Serializable]
    public class Variable
    {
        [SerializeField]
        protected string name = "";

        [SerializeField]
        protected UnityEngine.Object objectValue;

        [SerializeField]
        protected string dataValue;

        [SerializeField]
        protected VariableType variableType;

        public virtual string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        public virtual VariableType VariableType
        {
            get { return this.variableType; }
        }

        public virtual System.Type ValueType
        {
            get
            {
                switch (this.variableType)
                {
                    case VariableType.Boolean:
                        return typeof(bool);
                    case VariableType.Float:
                        return typeof(float);
                    case VariableType.Integer:
                        return typeof(int);
                    case VariableType.String:
                        return typeof(string);
                    case VariableType.Color:
                        return typeof(Color);
                    case VariableType.Vector2:
                        return typeof(Vector2);
                    case VariableType.Vector3:
                        return typeof(Vector3);
                    case VariableType.Vector4:
                        return typeof(Vector4);
                    case VariableType.Object:
                        return this.objectValue == null ? typeof(UnityEngine.Object) : this.objectValue.GetType();
                    case VariableType.GameObject:
                        return this.objectValue == null ? typeof(GameObject) : this.objectValue.GetType();
                    case VariableType.Component:
                        return this.objectValue == null ? typeof(Component) : this.objectValue.GetType();
                    default:
                        throw new System.NotSupportedException();
                }
            }
        }

        public virtual void SetValue<T>(T value)
        {
            this.SetValue(value);
        }

        public virtual T GetValue<T>()
        {
            return (T)GetValue();
        }

        public virtual void SetValue(object value)
        {
            switch (this.variableType)
            {
                case VariableType.Boolean:
                    this.dataValue = DataConverter.GetString((bool)value);
                    break;
                case VariableType.Float:
                    this.dataValue = DataConverter.GetString((float)value);
                    break;
                case VariableType.Integer:
                    this.dataValue = DataConverter.GetString((int)value);
                    break;
                case VariableType.String:
                    this.dataValue = DataConverter.GetString((string)value);
                    break;
                case VariableType.Color:
                    this.dataValue = DataConverter.GetString((Color)value);
                    break;
                case VariableType.Vector2:
                    this.dataValue = DataConverter.GetString((Vector2)value);
                    break;
                case VariableType.Vector3:
                    this.dataValue = DataConverter.GetString((Vector3)value);
                    break;
                case VariableType.Vector4:
                    this.dataValue = DataConverter.GetString((Vector4)value);
                    break;
                case VariableType.Object:
                    this.objectValue = (UnityEngine.Object)value;
                    break;
                case VariableType.GameObject:
                    this.objectValue = (GameObject)value;
                    break;
                case VariableType.Component:
                    this.objectValue = (Component)value;
                    break;
                default:
                    throw new System.NotSupportedException();
            }
        }
        public virtual object GetValue()
        {
            switch (this.variableType)
            {
                case VariableType.Boolean:
                    return DataConverter.ToBoolean(this.dataValue);
                case VariableType.Float:
                    return DataConverter.ToSingle(this.dataValue);
                case VariableType.Integer:
                    return DataConverter.ToInt32(this.dataValue);
                case VariableType.String:
                    return DataConverter.ToString(this.dataValue);
                case VariableType.Color:
                    return DataConverter.ToColor(this.dataValue);
                case VariableType.Vector2:
                    return DataConverter.ToVector2(this.dataValue);
                case VariableType.Vector3:
                    return DataConverter.ToVector3(this.dataValue);
                case VariableType.Vector4:
                    return DataConverter.ToVector4(this.dataValue);
                case VariableType.Object:
                    return this.objectValue;
                case VariableType.GameObject:
                    return this.objectValue;
                case VariableType.Component:
                    return this.objectValue;
                default:
                    throw new System.NotSupportedException();
            }
        }
    }
}
