var MusicTab = MusicTab || {};

// namespace function
MusicTab.namespace = function (nsString) {
    var parts = nsString.split('.'),
    parent = MusicTab,
    i;
    // strip redundant leading global
    if (parts[0] === "MusicTab") {
        parts = parts.slice(1);
    }
    for (i = 0; i < parts.length; i += 1) {
        // create a property if it doesn't exist
        if (typeof parent[parts[i]] === "undefined") {
            parent[parts[i]] = {};
        }
        parent = parent[parts[i]];
    }
    return parent;
};

// global klass function
klass = function (parent, props) {
    var child, f, i;
    // 1.
    // new constructor
    child = function () {
        if (child.uber && child.uber.hasOwnProperty("__construct")) {
            child.uber.__construct.apply(this, arguments);
        }
        if (child.prototype.hasOwnProperty("__construct")) {
            child.prototype.__construct.apply(this, arguments);
        }
    };
    // 2.
    // inherit
    parent = parent || Object;
    f = function () { };
    f.prototype = parent.prototype;
    child.prototype = new f();
    child.uber = parent.prototype;
    child.prototype.constructor = child;
    // 3.
    // add implementation methods
    for (i in props) {
        if (props.hasOwnProperty(i)) {
            child.prototype[i] = props[i];
        }
    }
    // return the "class"
    return child;
};

mix = function() {
    var arg, prop, child = { };
    for (arg = 0; arg < arguments.length; arg += 1) {
        for (prop in arguments[arg]) {
            if (arguments[arg].hasOwnProperty(prop)) {
                child[prop] = arguments[arg][prop];
            }
        }
    }
    return child;

};

String.prototype.endsWith = function (suffix) {
    return this.indexOf(suffix, this.length - suffix.length) !== -1;
};

