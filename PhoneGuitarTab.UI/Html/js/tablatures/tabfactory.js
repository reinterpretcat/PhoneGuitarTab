MusicTab.namespace('MusicTab.Tablatures');

MusicTab.Tablatures.TabFactory = klass(null, {
    __construct: function (params, success, error) {
        this.title = params.title || "";
        this.type = params.type;
        this.helper = params.helper;
        this.init(params.data, success);
    },
    
    init: function (data, success) {
        
        if (this.type == "ug_json") {
            this._jsonTab(data, success);
        } else {
            this._alphaTab(data, success);
        }
    },   

	// TODO remove platform-specific logic
    _jsonTab: function (content, success) {
        MusicTab.Tablatures.UgTabAdapter.transform(this.title, JSON.parse(content), this.helper, success);
    },

    _alphaTab: function (data, success) {
		var closure = this.helper;
		var factory = new alphatab.tablature.model.DrawingSongModelFactory();
		alphatab.file.SongLoader.loadSong(data, factory, function (song) {
			MusicTab.Tablatures.AlphaTabAdapter.transform(song, closure, success);
		});      
    }
});

MusicTab.Tablatures.TabFactory.create = function (params, success, error) {
    new MusicTab.Tablatures.TabFactory(params, success, error);
}