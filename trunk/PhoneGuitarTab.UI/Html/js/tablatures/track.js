MusicTab.namespace('MusicTab.Tablatures');

MusicTab.Tablatures.Track = klass(null, {
__construct: function (params) {
		this.init(params || {});
    },
	
	init: function(params){
		this.measures = params.measures || [];
		this.index = params.index;
		this.name = params.name;
		this.instrument = params.instrument;
		this.strings = params.strings || [];
		this.selected = params.selected;
	}
});