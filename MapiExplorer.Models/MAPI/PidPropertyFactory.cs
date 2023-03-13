namespace MapiExplorer.Models.MAPI
{
    public static class PidPropertyFactory
    {
        public static PidTagProperty Create(PidTagProperties pidTag)
        {
            return new PidTagProperty
            {
                Name = pidTag,
                Type = PidTagHelper.PidTagTypes[pidTag]
            };
        }

        public static PidLidProperty Create(PidLidProperties pidLid)
        {
            return new PidLidProperty
            {
                Name = pidLid,
                Type = PidLidHelper.PidLidTypes[pidLid],
                PropertySet = PidLidHelper.PidLidPropertySets[pidLid]
            };
        }

        public static PidNameProperty Create(PidNameProperties pidName)
        {
            return new PidNameProperty
            {
                Id = $"{pidName}",
                Name = PidNameHelper.PidNameAlternateName[pidName],
                Type = PidNameHelper.PidNameTypes[pidName],
                PropertySet = PidNameHelper.PidNameNamespace[pidName]
            };
        }
    }
}