MusicTab.namespace('MusicTab.Stave.Context');

MusicTab.Stave.Context = klass(null, {
    __construct: function(params) {
        this.init(params);
    },

    init: function (params) {
        this.scale = params.scale;
        this.height = params.height;
        this.width = params.width;
        this.placeHolderId = params.placeHolderId; //s3-flexbox
        this.tabDivClass = params.tabDivClass; //vex-tabdiv
    }
});