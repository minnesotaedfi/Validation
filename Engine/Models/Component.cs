namespace Engine.Models
{
    public struct Component
    {
        public string ComponentName { get; }
        public string CharacteristicName { get; }

        public Component(string componentName, string characteristicName = null)
        {
            ComponentName = componentName;
            CharacteristicName = characteristicName;
        }

        public override string ToString()
        {
            return $"{{{ComponentName}}}.[{CharacteristicName}]";
        }
    }
}
