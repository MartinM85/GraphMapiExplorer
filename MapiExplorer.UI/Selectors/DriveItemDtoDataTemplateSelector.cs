using MapiExplorer.Models;

namespace MapiExplorer.UI.Selectors
{
    public class DriveItemDtoDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate FileTemplate { get; set; }
        public DataTemplate FolderTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            return ((DriveItemDto)item).IsFile ? FileTemplate : FolderTemplate;
        }
    }
}
