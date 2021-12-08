/// <reference types="@mapeditor/tiled-api" />


const action = tiled.registerAction("CleanAllFilePaths", function (action) {
    if (tiled.activeAsset === null) {
        return;
    }
    const asset = tiled.activeAsset;
    const map = asset as TileMap;
    tiled.log("Cleaning all file paths...");
    map.selectedObjects.forEach(obj => {
        //tiled.log(`Cleaning file path of ${obj.id}`);
        const properties = obj.resolvedProperties();
        for (const key in properties) {
            if (properties.hasOwnProperty(key)) {
                const property = properties[key].toString();
                if (property.includes(":/")) {
                    
                    const firstpass = property.replace(/^.*addons\//, "");
                    const matpath_clean = firstpass.substring(firstpass.indexOf("/") + 1);
                    const filep = tiled.filePath(matpath_clean);

                    //tiled.log(`Cleaned file path: ${property}`);
                    obj.setProperty(key, filep);
                } 
            }
        }
        
        
        
        const matpath = properties["CustomMaterial"];
        //remove everything before addons in the matpath
       //const matpath_clean = matpath..replace(/^.*addons\//, "");
       //tiled.log(`Cleaned file path: ${matpath_clean}`);
       //properties["CustomMaterial"] = matpath_clean;
    
    });
})
action.text = "Clean File Paths";
action.checkable = true;
tiled.assetAboutToBeSaved.connect(function (asset) {
    tiled.trigger("SelectAll");
    tiled.trigger("CleanAllFilePaths");
    tiled.trigger("SelectNone");
});

tiled.extendMenu("Edit", [
    { action: "CleanAllFilePaths", before: "SelectAll" },
    { separator: true },
])

