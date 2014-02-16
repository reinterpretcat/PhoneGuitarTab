MusicTab.namespace('MusicTab.Tablatures');

MusicTab.Tablatures.Title = klass(null, {
__construct: function (params) {
		this.init(params || {});
    },
	
	init: function(params){
		this.title = params.title || "";
		this.artist = params.artist|| "";
		this.album = params.album|| "";
		this.instrument = params.instrument|| "";
	}
});
