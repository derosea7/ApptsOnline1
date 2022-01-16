
/**
 * Author: Adam James DeRose
 * Date: 1010/2019
 * Time zone related funcationality.
 * Requires parital HTML view to function. _TimeZoneDisplayPartial.cshtml
 */
var appts_tz = (function () {
  var p = {}, _p = {};
  _p.test = {};

  p.init = function (options) {
    _p.handle.register();
    _p.initByOptions(options);
  };

  _p.initByOptions = function (options) {
    console.log('otions from tz2', options);
    _p.handle.onChange.timeZoneChanged(options.onTimeZoneChanged);

    if (options.presetTimeZoneId !== undefined && options.presetTimeZoneId !== '') {
      _p.handle.onLoad.setTimeZoneInfo(options.presetTimeZoneId, options.onTimeZoneChanged);
    } else {
      var detectedTz = moment.tz.guess();
      _p.handle.onLoad.setTimeZoneInfo(detectedTz, options.onTimeZoneChanged);
    }

  };

  /// <summary>
  /// Container to define event handlers.
  /// </summary>
  _p.handle = {
    onChange: {

      timeZoneChanged: function (completed) {
        $('#ddl-timezone-picker').on('change', function (el) {
          var selectedTimeZone = $(el.target).val();
          var selectedTimeZoneText = $(this).find("option:selected").text();
          _p.setTimeZoneInfo(selectedTimeZone, selectedTimeZoneText, completed);
        });
      },

      loadTimeZonesWhenRegionChanged: function () {
        $('#ddl-timezone-regions').on('change', function (el) {
          var region = $(el.target).val();
          _p.setTimeZoneDdlByRegion(region);
        });
      }

    },
    onLoad: {

      setTimeZoneInfo: function (ianaTimeZoneId, completed) {
        _p.setTimeZoneInfo(ianaTimeZoneId, ianaTimeZoneId, completed);
        _p.initDropDowns(ianaTimeZoneId);
      }
    }
  };

  /// <summary>
  /// Register all event handlers.
  /// </summary>
  _p.handle.register = function () {
    _p.handle.onChange.loadTimeZonesWhenRegionChanged();
  };

  /// <summary>
  /// Saves time zone to hidden field on page.
  /// </summary>
  /// <params name="timeZoneId"></params>
  /// <params name="onTimeZoneSet">
  /// Callback function executed when time zone information is available.
  /// </params >
  _p.setTimeZoneInfo = function (timeZoneId, timeZoneText, onTimeZoneSet) {
    var m = moment.tz(timeZoneId);
    var formattedTime = m.format('LT');
    $('#current-time-on-form').html(formattedTime);
    $('#current-time').html(formattedTime);
    $('#current-timezone').html(timeZoneText);

    if (onTimeZoneSet !== undefined) {
      onTimeZoneSet(timeZoneId, formattedTime);
    }
  };
  _p.initDropDowns = function (timeZone) {
    var region = timeZone.split('/')[0];
    var regionddl = '#ddl-timezone-regions option[value="' + region + '"]';
    $(regionddl).prop('selected', true);
    _p.setTimeZoneDdlByRegion(region);
    var tzddl = '#ddl-timezone-picker option[value="' + timeZone + '"]';
    $(tzddl).prop('selected', true);
  };

  /// <summary>
  /// Given an array of time zone data objects, this function
  /// returns an option list for select element.
  /// </summary>
  /// <params name="timeZoneData">Array of time zone objects</params>
  /// <returns>Html of an option list to be put into select element</returns>
  _p.createTimeZoneOptionList = function (timeZoneData) {
    var html = [];
    for (var i = 0; i < timeZoneData.length; i++) {
      html.push('<option value="', timeZoneData[i].timezone, '" >', timeZoneData[i].display, '</option>');
    }
    return html.join('');
  };

  /// <summary>
  /// Given list of time zone data objects, sets the timezone drop down list
  /// with the options from the list.
  /// </summary>
  _p.setTimeZoneDdl = function (timeZoneData) {
    $('#ddl-timezone-picker').html(_p.createTimeZoneOptionList(timeZoneData));
  };

  /// <summary>
  /// Given a region, sets the drop down list of corresponding timezones.
  /// </summary>
  /// <params name="region">
  /// US, America, Europe, etc.
  /// </params>
  _p.setTimeZoneDdlByRegion = function (region) {
    switch (region) {
      case 'US':
        _p.setTimeZoneDdl(_p.timeZoneData.us());
        break;

      case 'America':
        _p.setTimeZoneDdl(_p.timeZoneData.america());
        break;

      case 'Africa':
        _p.setTimeZoneDdl(_p.timeZoneData.africa());
        break;

      case 'Asia':
        _p.setTimeZoneDdl(_p.timeZoneData.africa());
        break;


      case 'Atlantic':
        _p.setTimeZoneDdl(_p.timeZoneData.atlantic());
        break;


      case 'Australia':
        _p.setTimeZoneDdl(_p.timeZoneData.australia());
        break;


      case 'Canada':
        _p.setTimeZoneDdl(_p.timeZoneData.canada());
        break;


      case 'Europe':
        _p.setTimeZoneDdl(_p.timeZoneData.europe());
        break;

      case 'Indian':
        _p.setTimeZoneDdl(_p.timeZoneData.indian());
        break;

      case 'Pacific':
        _p.setTimeZoneDdl(_p.timeZoneData.pacific());
        break;

      default:
        break;
    }
  };

  /// <summary>
  /// Container for all time zone drop down data.
  /// </summary>
  _p.timeZoneData = {};

  _p.timeZoneData.us = function () {
    return [
      { group: "", timezone: "US/Alaska", display: "Alaska" },
      { group: "", timezone: "US/Arizona", display: "Arizona" },
      { group: "", timezone: "US/Central", display: "Central" },
      { group: "", timezone: "US/Eastern", display: "Eastern" },
      { group: "", timezone: "US/Hawaii", display: "Hawaii" },
      { group: "", timezone: "US/Mountain", display: "Mountain" },
      { group: "", timezone: "US/Pacific", display: "Pacific" }
    ];
  };

  _p.timeZoneData.america = function () {
    return [
      { group: "", timezone: "America/Adak", display: "Adak" },
      { group: "", timezone: "America/Anchorage", display: "Anchorage" },
      { group: "", timezone: "America/Anguilla", display: "Anguilla" },
      { group: "", timezone: "America/Antigua", display: "Antigua" },
      { group: "", timezone: "America/Araguaina", display: "Araguaina" },
      { group: "", timezone: "America/Argentina/Buenos_Aires", display: "Argentina/Buenos Aires" },
      { group: "", timezone: "America/Argentina/Catamarca", display: "Argentina/Catamarca" },
      { group: "", timezone: "America/Argentina/Cordoba", display: "Argentina/Cordoba" },
      { group: "", timezone: "America/Argentina/Jujuy", display: "Argentina/Jujuy" },
      { group: "", timezone: "America/Argentina/La_Rioja", display: "Argentina/La Rioja" },
      { group: "", timezone: "America/Argentina/Mendoza", display: "Argentina/Mendoza" },
      { group: "", timezone: "America/Argentina/Rio_Gallegos", display: "Argentina/Rio Gallegos" },
      { group: "", timezone: "America/Argentina/Salta", display: "Argentina/Salta" },
      { group: "", timezone: "America/Argentina/San_Juan", display: "Argentina/San Juan" },
      { group: "", timezone: "America/Argentina/San_Luis", display: "Argentina/San Luis" },
      { group: "", timezone: "America/Argentina/Tucuman", display: "Argentina/Tucuman" },
      { group: "", timezone: "America/Argentina/Ushuaia", display: "Argentina/Ushuaia" },
      { group: "", timezone: "America/Aruba", display: "Aruba" },
      { group: "", timezone: "America/Asuncion", display: "Asuncion" },
      { group: "", timezone: "America/Atikokan", display: "Atikokan" },
      { group: "", timezone: "America/Bahia", display: "Bahia" },
      { group: "", timezone: "America/Bahia_Banderas", display: "Bahia Banderas" },
      { group: "", timezone: "America/Barbados", display: "Barbados" },
      { group: "", timezone: "America/Belem", display: "Belem" },
      { group: "", timezone: "America/Belize", display: "Belize" },
      { group: "", timezone: "America/Blanc-Sablon", display: "Blanc-Sablon" },
      { group: "", timezone: "America/Boa_Vista", display: "Boa Vista" },
      { group: "", timezone: "America/Bogota", display: "Bogota" },
      { group: "", timezone: "America/Boise", display: "Boise" },
      { group: "", timezone: "America/Cambridge_Bay", display: "Cambridge Bay" },
      { group: "", timezone: "America/Campo_Grande", display: "Campo Grande" },
      { group: "", timezone: "America/Cancun", display: "Cancun" },
      { group: "", timezone: "America/Caracas", display: "Caracas" },
      { group: "", timezone: "America/Cayenne", display: "Cayenne" },
      { group: "", timezone: "America/Cayman", display: "Cayman" },
      { group: "", timezone: "America/Chicago", display: "Chicago" },
      { group: "", timezone: "America/Chihuahua", display: "Chihuahua" },
      { group: "", timezone: "America/Costa_Rica", display: "Costa Rica" },
      { group: "", timezone: "America/Creston", display: "Creston" },
      { group: "", timezone: "America/Cuiaba", display: "Cuiaba" },
      { group: "", timezone: "America/Curacao", display: "Curacao" },
      { group: "", timezone: "America/Danmarkshavn", display: "Danmarkshavn" },
      { group: "", timezone: "America/Dawson", display: "Dawson" },
      { group: "", timezone: "America/Dawson_Creek", display: "Dawson Creek" },
      { group: "", timezone: "America/Denver", display: "Denver" },
      { group: "", timezone: "America/Detroit", display: "Detroit" },
      { group: "", timezone: "America/Dominica", display: "Dominica" },
      { group: "", timezone: "America/Edmonton", display: "Edmonton" },
      { group: "", timezone: "America/Eirunepe", display: "Eirunepe" },
      { group: "", timezone: "America/El_Salvador", display: "El Salvador" },
      { group: "", timezone: "America/Fort_Nelson", display: "Fort Nelson" },
      { group: "", timezone: "America/Fortaleza", display: "Fortaleza" },
      { group: "", timezone: "America/Glace_Bay", display: "Glace Bay" },
      { group: "", timezone: "America/Godthab", display: "Godthab" },
      { group: "", timezone: "America/Goose_Bay", display: "Goose Bay" },
      { group: "", timezone: "America/Grand_Turk", display: "Grand Turk" },
      { group: "", timezone: "America/Grenada", display: "Grenada" },
      { group: "", timezone: "America/Guadeloupe", display: "Guadeloupe" },
      { group: "", timezone: "America/Guatemala", display: "Guatemala" },
      { group: "", timezone: "America/Guayaquil", display: "Guayaquil" },
      { group: "", timezone: "America/Guyana", display: "Guyana" },
      { group: "", timezone: "America/Halifax", display: "Halifax" },
      { group: "", timezone: "America/Havana", display: "Havana" },
      { group: "", timezone: "America/Hermosillo", display: "Hermosillo" },
      { group: "", timezone: "America/Indiana/Indianapolis", display: "Indiana/Indianapolis" },
      { group: "", timezone: "America/Indiana/Knox", display: "Indiana/Knox" },
      { group: "", timezone: "America/Indiana/Marengo", display: "Indiana/Marengo" },
      { group: "", timezone: "America/Indiana/Petersburg", display: "Indiana/Petersburg" },
      { group: "", timezone: "America/Indiana/Tell_City", display: "Indiana/Tell City" },
      { group: "", timezone: "America/Indiana/Vevay", display: "Indiana/Vevay" },
      { group: "", timezone: "America/Indiana/Vincennes", display: "Indiana/Vincennes" },
      { group: "", timezone: "America/Indiana/Winamac", display: "Indiana/Winamac" },
      { group: "", timezone: "America/Inuvik", display: "Inuvik" },
      { group: "", timezone: "America/Iqaluit", display: "Iqaluit" },
      { group: "", timezone: "America/Jamaica", display: "Jamaica" },
      { group: "", timezone: "America/Juneau", display: "Juneau" },
      { group: "", timezone: "America/Kentucky/Louisville", display: "Kentucky/Louisville" },
      { group: "", timezone: "America/Kentucky/Monticello", display: "Kentucky/Monticello" },
      { group: "", timezone: "America/Kralendijk", display: "Kralendijk" },
      { group: "", timezone: "America/La_Paz", display: "La Paz" },
      { group: "", timezone: "America/Lima", display: "Lima" },
      { group: "", timezone: "America/Los_Angeles", display: "Los Angeles" },
      { group: "", timezone: "America/Lower_Princes", display: "Lower Princes" },
      { group: "", timezone: "America/Maceio", display: "Maceio" },
      { group: "", timezone: "America/Managua", display: "Managua" },
      { group: "", timezone: "America/Manaus", display: "Manaus" },
      { group: "", timezone: "America/Marigot", display: "Marigot" },
      { group: "", timezone: "America/Martinique", display: "Martinique" },
      { group: "", timezone: "America/Matamoros", display: "Matamoros" },
      { group: "", timezone: "America/Mazatlan", display: "Mazatlan" },
      { group: "", timezone: "America/Menominee", display: "Menominee" },
      { group: "", timezone: "America/Merida", display: "Merida" },
      { group: "", timezone: "America/Metlakatla", display: "Metlakatla" },
      { group: "", timezone: "America/Mexico_City", display: "Mexico City" },
      { group: "", timezone: "America/Miquelon", display: "Miquelon" },
      { group: "", timezone: "America/Moncton", display: "Moncton" },
      { group: "", timezone: "America/Monterrey", display: "Monterrey" },
      { group: "", timezone: "America/Montevideo", display: "Montevideo" },
      { group: "", timezone: "America/Montserrat", display: "Montserrat" },
      { group: "", timezone: "America/Nassau", display: "Nassau" },
      { group: "", timezone: "America/New_York", display: "New York" },
      { group: "", timezone: "America/Nipigon", display: "Nipigon" },
      { group: "", timezone: "America/Nome", display: "Nome" },
      { group: "", timezone: "America/Noronha", display: "Noronha" },
      { group: "", timezone: "America/North_Dakota/Beulah", display: "North Dakota/Beulah" },
      { group: "", timezone: "America/North_Dakota/Center", display: "North Dakota/Center" },
      { group: "", timezone: "America/North_Dakota/New_Salem", display: "North Dakota/New Salem" },
      { group: "", timezone: "America/Ojinaga", display: "Ojinaga" },
      { group: "", timezone: "America/Panama", display: "Panama" },
      { group: "", timezone: "America/Pangnirtung", display: "Pangnirtung" },
      { group: "", timezone: "America/Paramaribo", display: "Paramaribo" },
      { group: "", timezone: "America/Phoenix", display: "Phoenix" },
      { group: "", timezone: "America/Port-au-Prince", display: "Port-au-Prince" },
      { group: "", timezone: "America/Port_of_Spain", display: "Port of Spain" },
      { group: "", timezone: "America/Porto_Velho", display: "Porto Velho" },
      { group: "", timezone: "America/Puerto_Rico", display: "Puerto Rico" },
      { group: "", timezone: "America/Punta_Arenas", display: "Punta Arenas" },
      { group: "", timezone: "America/Rainy_River", display: "Rainy River" },
      { group: "", timezone: "America/Rankin_Inlet", display: "Rankin Inlet" },
      { group: "", timezone: "America/Recife", display: "Recife" },
      { group: "", timezone: "America/Regina", display: "Regina" },
      { group: "", timezone: "America/Resolute", display: "Resolute" },
      { group: "", timezone: "America/Rio_Branco", display: "Rio Branco" },
      { group: "", timezone: "America/Santarem", display: "Santarem" },
      { group: "", timezone: "America/Santiago", display: "Santiago" },
      { group: "", timezone: "America/Santo_Domingo", display: "Santo Domingo" },
      { group: "", timezone: "America/Sao_Paulo", display: "Sao Paulo" },
      { group: "", timezone: "America/Scoresbysund", display: "Scoresbysund" },
      { group: "", timezone: "America/Sitka", display: "Sitka" },
      { group: "", timezone: "America/St_Barthelemy", display: "St Barthelemy" },
      { group: "", timezone: "America/St_Johns", display: "St Johns" },
      { group: "", timezone: "America/St_Kitts", display: "St Kitts" },
      { group: "", timezone: "America/St_Lucia", display: "St Lucia" },
      { group: "", timezone: "America/St_Thomas", display: "St Thomas" },
      { group: "", timezone: "America/St_Vincent", display: "St Vincent" },
      { group: "", timezone: "America/Swift_Current", display: "Swift Current" },
      { group: "", timezone: "America/Tegucigalpa", display: "Tegucigalpa" },
      { group: "", timezone: "America/Thule", display: "Thule" },
      { group: "", timezone: "America/Thunder_Bay", display: "Thunder Bay" },
      { group: "", timezone: "America/Tijuana", display: "Tijuana" },
      { group: "", timezone: "America/Toronto", display: "Toronto" },
      { group: "", timezone: "America/Tortola", display: "Tortola" },
      { group: "", timezone: "America/Vancouver", display: "Vancouver" },
      { group: "", timezone: "America/Whitehorse", display: "Whitehorse" },
      { group: "", timezone: "America/Winnipeg", display: "Winnipeg" },
      { group: "", timezone: "America/Yakutat", display: "Yakutat" },
      { group: "", timezone: "America/Yellowknife", display: "Yellowknife" }
    ];
  };

  _p.timeZoneData.africa = function () {
    return [
      { group: "", timezone: "Africa/Abidjan", display: "Abidjan" },
      { group: "", timezone: "Africa/Accra", display: "Accra" },
      { group: "", timezone: "Africa/Addis_Ababa", display: "Addis Ababa" },
      { group: "", timezone: "Africa/Algiers", display: "Algiers" },
      { group: "", timezone: "Africa/Asmara", display: "Asmara" },
      { group: "", timezone: "Africa/Bamako", display: "Bamako" },
      { group: "", timezone: "Africa/Bangui", display: "Bangui" },
      { group: "", timezone: "Africa/Banjul", display: "Banjul" },
      { group: "", timezone: "Africa/Bissau", display: "Bissau" },
      { group: "", timezone: "Africa/Blantyre", display: "Blantyre" },
      { group: "", timezone: "Africa/Brazzaville", display: "Brazzaville" },
      { group: "", timezone: "Africa/Bujumbura", display: "Bujumbura" },
      { group: "", timezone: "Africa/Cairo", display: "Cairo" },
      { group: "", timezone: "Africa/Casablanca", display: "Casablanca" },
      { group: "", timezone: "Africa/Ceuta", display: "Ceuta" },
      { group: "", timezone: "Africa/Conakry", display: "Conakry" },
      { group: "", timezone: "Africa/Dakar", display: "Dakar" },
      { group: "", timezone: "Africa/Dar_es_Salaam", display: "Dar es Salaam" },
      { group: "", timezone: "Africa/Djibouti", display: "Djibouti" },
      { group: "", timezone: "Africa/Douala", display: "Douala" },
      { group: "", timezone: "Africa/El_Aaiun", display: "El Aaiun" },
      { group: "", timezone: "Africa/Freetown", display: "Freetown" },
      { group: "", timezone: "Africa/Gaborone", display: "Gaborone" },
      { group: "", timezone: "Africa/Harare", display: "Harare" },
      { group: "", timezone: "Africa/Johannesburg", display: "Johannesburg" },
      { group: "", timezone: "Africa/Juba", display: "Juba" },
      { group: "", timezone: "Africa/Kampala", display: "Kampala" },
      { group: "", timezone: "Africa/Khartoum", display: "Khartoum" },
      { group: "", timezone: "Africa/Kigali", display: "Kigali" },
      { group: "", timezone: "Africa/Kinshasa", display: "Kinshasa" },
      { group: "", timezone: "Africa/Lagos", display: "Lagos" },
      { group: "", timezone: "Africa/Libreville", display: "Libreville" },
      { group: "", timezone: "Africa/Lome", display: "Lome" },
      { group: "", timezone: "Africa/Luanda", display: "Luanda" },
      { group: "", timezone: "Africa/Lubumbashi", display: "Lubumbashi" },
      { group: "", timezone: "Africa/Lusaka", display: "Lusaka" },
      { group: "", timezone: "Africa/Malabo", display: "Malabo" },
      { group: "", timezone: "Africa/Maputo", display: "Maputo" },
      { group: "", timezone: "Africa/Maseru", display: "Maseru" },
      { group: "", timezone: "Africa/Mbabane", display: "Mbabane" },
      { group: "", timezone: "Africa/Mogadishu", display: "Mogadishu" },
      { group: "", timezone: "Africa/Monrovia", display: "Monrovia" },
      { group: "", timezone: "Africa/Nairobi", display: "Nairobi" },
      { group: "", timezone: "Africa/Ndjamena", display: "Ndjamena" },
      { group: "", timezone: "Africa/Niamey", display: "Niamey" },
      { group: "", timezone: "Africa/Nouakchott", display: "Nouakchott" },
      { group: "", timezone: "Africa/Ouagadougou", display: "Ouagadougou" },
      { group: "", timezone: "Africa/Porto-Novo", display: "Porto-Novo" },
      { group: "", timezone: "Africa/Sao_Tome", display: "Sao Tome" },
      { group: "", timezone: "Africa/Tripoli", display: "Tripoli" },
      { group: "", timezone: "Africa/Tunis", display: "Tunis" },
      { group: "", timezone: "Africa/Windhoek", display: "Windhoek" }
    ];
  };

  _p.timeZoneData.asia = function () {
    return [
      { group: "", timezone: "Asia/Aden", display: "Aden" },
      { group: "", timezone: "Asia/Almaty", display: "Almaty" },
      { group: "", timezone: "Asia/Amman", display: "Amman" },
      { group: "", timezone: "Asia/Anadyr", display: "Anadyr" },
      { group: "", timezone: "Asia/Aqtau", display: "Aqtau" },
      { group: "", timezone: "Asia/Aqtobe", display: "Aqtobe" },
      { group: "", timezone: "Asia/Ashgabat", display: "Ashgabat" },
      { group: "", timezone: "Asia/Atyrau", display: "Atyrau" },
      { group: "", timezone: "Asia/Baghdad", display: "Baghdad" },
      { group: "", timezone: "Asia/Bahrain", display: "Bahrain" },
      { group: "", timezone: "Asia/Baku", display: "Baku" },
      { group: "", timezone: "Asia/Bangkok", display: "Bangkok" },
      { group: "", timezone: "Asia/Barnaul", display: "Barnaul" },
      { group: "", timezone: "Asia/Beirut", display: "Beirut" },
      { group: "", timezone: "Asia/Bishkek", display: "Bishkek" },
      { group: "", timezone: "Asia/Brunei", display: "Brunei" },
      { group: "", timezone: "Asia/Chita", display: "Chita" },
      { group: "", timezone: "Asia/Choibalsan", display: "Choibalsan" },
      { group: "", timezone: "Asia/Colombo", display: "Colombo" },
      { group: "", timezone: "Asia/Damascus", display: "Damascus" },
      { group: "", timezone: "Asia/Dhaka", display: "Dhaka" },
      { group: "", timezone: "Asia/Dili", display: "Dili" },
      { group: "", timezone: "Asia/Dubai", display: "Dubai" },
      { group: "", timezone: "Asia/Dushanbe", display: "Dushanbe" },
      { group: "", timezone: "Asia/Famagusta", display: "Famagusta" },
      { group: "", timezone: "Asia/Gaza", display: "Gaza" },
      { group: "", timezone: "Asia/Hebron", display: "Hebron" },
      { group: "", timezone: "Asia/Ho_Chi_Minh", display: "Ho Chi Minh" },
      { group: "", timezone: "Asia/Hong_Kong", display: "Hong Kong" },
      { group: "", timezone: "Asia/Hovd", display: "Hovd" },
      { group: "", timezone: "Asia/Irkutsk", display: "Irkutsk" },
      { group: "", timezone: "Asia/Jakarta", display: "Jakarta" },
      { group: "", timezone: "Asia/Jayapura", display: "Jayapura" },
      { group: "", timezone: "Asia/Jerusalem", display: "Jerusalem" },
      { group: "", timezone: "Asia/Kabul", display: "Kabul" },
      { group: "", timezone: "Asia/Kamchatka", display: "Kamchatka" },
      { group: "", timezone: "Asia/Karachi", display: "Karachi" },
      { group: "", timezone: "Asia/Kathmandu", display: "Kathmandu" },
      { group: "", timezone: "Asia/Khandyga", display: "Khandyga" },
      { group: "", timezone: "Asia/Kolkata", display: "Kolkata" },
      { group: "", timezone: "Asia/Krasnoyarsk", display: "Krasnoyarsk" },
      { group: "", timezone: "Asia/Kuala_Lumpur", display: "Kuala Lumpur" },
      { group: "", timezone: "Asia/Kuching", display: "Kuching" },
      { group: "", timezone: "Asia/Kuwait", display: "Kuwait" },
      { group: "", timezone: "Asia/Macau", display: "Macau" },
      { group: "", timezone: "Asia/Magadan", display: "Magadan" },
      { group: "", timezone: "Asia/Makassar", display: "Makassar" },
      { group: "", timezone: "Asia/Manila", display: "Manila" },
      { group: "", timezone: "Asia/Muscat", display: "Muscat" },
      { group: "", timezone: "Asia/Nicosia", display: "Nicosia" },
      { group: "", timezone: "Asia/Novokuznetsk", display: "Novokuznetsk" },
      { group: "", timezone: "Asia/Novosibirsk", display: "Novosibirsk" },
      { group: "", timezone: "Asia/Omsk", display: "Omsk" },
      { group: "", timezone: "Asia/Oral", display: "Oral" },
      { group: "", timezone: "Asia/Phnom_Penh", display: "Phnom Penh" },
      { group: "", timezone: "Asia/Pontianak", display: "Pontianak" },
      { group: "", timezone: "Asia/Pyongyang", display: "Pyongyang" },
      { group: "", timezone: "Asia/Qatar", display: "Qatar" },
      { group: "", timezone: "Asia/Qostanay", display: "Qostanay" },
      { group: "", timezone: "Asia/Qyzylorda", display: "Qyzylorda" },
      { group: "", timezone: "Asia/Riyadh", display: "Riyadh" },
      { group: "", timezone: "Asia/Sakhalin", display: "Sakhalin" },
      { group: "", timezone: "Asia/Samarkand", display: "Samarkand" },
      { group: "", timezone: "Asia/Seoul", display: "Seoul" },
      { group: "", timezone: "Asia/Shanghai", display: "Shanghai" },
      { group: "", timezone: "Asia/Singapore", display: "Singapore" },
      { group: "", timezone: "Asia/Srednekolymsk", display: "Srednekolymsk" },
      { group: "", timezone: "Asia/Taipei", display: "Taipei" },
      { group: "", timezone: "Asia/Tashkent", display: "Tashkent" },
      { group: "", timezone: "Asia/Tbilisi", display: "Tbilisi" },
      { group: "", timezone: "Asia/Tehran", display: "Tehran" },
      { group: "", timezone: "Asia/Thimphu", display: "Thimphu" },
      { group: "", timezone: "Asia/Tokyo", display: "Tokyo" },
      { group: "", timezone: "Asia/Tomsk", display: "Tomsk" },
      { group: "", timezone: "Asia/Ulaanbaatar", display: "Ulaanbaatar" },
      { group: "", timezone: "Asia/Urumqi", display: "Urumqi" },
      { group: "", timezone: "Asia/Ust-Nera", display: "Ust-Nera" },
      { group: "", timezone: "Asia/Vientiane", display: "Vientiane" },
      { group: "", timezone: "Asia/Vladivostok", display: "Vladivostok" },
      { group: "", timezone: "Asia/Yakutsk", display: "Yakutsk" },
      { group: "", timezone: "Asia/Yangon", display: "Yangon" },
      { group: "", timezone: "Asia/Yekaterinburg", display: "Yekaterinburg" },
      { group: "", timezone: "Asia/Yerevan", display: "Yerevan" }
    ];
  };


  _p.timeZoneData.atlantic = function () {
    return [
      { group: "", timezone: "Atlantic/Azores", display: "Azores" },
      { group: "", timezone: "Atlantic/Bermuda", display: "Bermuda" },
      { group: "", timezone: "Atlantic/Canary", display: "Canary" },
      { group: "", timezone: "Atlantic/Cape_Verde", display: "Cape Verde" },
      { group: "", timezone: "Atlantic/Faroe", display: "Faroe" },
      { group: "", timezone: "Atlantic/Madeira", display: "Madeira" },
      { group: "", timezone: "Atlantic/Reykjavik", display: "Reykjavik" },
      { group: "", timezone: "Atlantic/South_Georgia", display: "South Georgia" },
      { group: "", timezone: "Atlantic/St_Helena", display: "St Helena" },
      { group: "", timezone: "Atlantic/Stanley", display: "Stanley" }
    ];
  };

  _p.timeZoneData.australia = function () {
    return [
      { group: "", timezone: "Australia/Adelaide", display: "Adelaide" },
      { group: "", timezone: "Australia/Brisbane", display: "Brisbane" },
      { group: "", timezone: "Australia/Broken_Hill", display: "Broken Hill" },
      { group: "", timezone: "Australia/Currie", display: "Currie" },
      { group: "", timezone: "Australia/Darwin", display: "Darwin" },
      { group: "", timezone: "Australia/Eucla", display: "Eucla" },
      { group: "", timezone: "Australia/Hobart", display: "Hobart" },
      { group: "", timezone: "Australia/Lindeman", display: "Lindeman" },
      { group: "", timezone: "Australia/Lord_Howe", display: "Lord Howe" },
      { group: "", timezone: "Australia/Melbourne", display: "Melbourne" },
      { group: "", timezone: "Australia/Perth", display: "Perth" },
      { group: "", timezone: "Australia/Sydney", display: "Sydney" }
    ];
  };

  _p.timeZoneData.canada = function () {
    return [
      { group: "", timezone: "Canada/Atlantic", display: "Atlantic" },
      { group: "", timezone: "Canada/Central", display: "Central" },
      { group: "", timezone: "Canada/Eastern", display: "Eastern" },
      { group: "", timezone: "Canada/Mountain", display: "Mountain" },
      { group: "", timezone: "Canada/Newfoundland", display: "Newfoundland" },
      { group: "", timezone: "Canada/Pacific", display: "Pacific" }
    ];
  };

  _p.timeZoneData.europe = function () {
    return [
      { group: "", timezone: "Europe/Amsterdam", display: "Amsterdam" },
      { group: "", timezone: "Europe/Andorra", display: "Andorra" },
      { group: "", timezone: "Europe/Astrakhan", display: "Astrakhan" },
      { group: "", timezone: "Europe/Athens", display: "Athens" },
      { group: "", timezone: "Europe/Belgrade", display: "Belgrade" },
      { group: "", timezone: "Europe/Berlin", display: "Berlin" },
      { group: "", timezone: "Europe/Bratislava", display: "Bratislava" },
      { group: "", timezone: "Europe/Brussels", display: "Brussels" },
      { group: "", timezone: "Europe/Bucharest", display: "Bucharest" },
      { group: "", timezone: "Europe/Budapest", display: "Budapest" },
      { group: "", timezone: "Europe/Busingen", display: "Busingen" },
      { group: "", timezone: "Europe/Chisinau", display: "Chisinau" },
      { group: "", timezone: "Europe/Copenhagen", display: "Copenhagen" },
      { group: "", timezone: "Europe/Dublin", display: "Dublin" },
      { group: "", timezone: "Europe/Gibraltar", display: "Gibraltar" },
      { group: "", timezone: "Europe/Guernsey", display: "Guernsey" },
      { group: "", timezone: "Europe/Helsinki", display: "Helsinki" },
      { group: "", timezone: "Europe/Isle_of_Man", display: "Isle of Man" },
      { group: "", timezone: "Europe/Istanbul", display: "Istanbul" },
      { group: "", timezone: "Europe/Jersey", display: "Jersey" },
      { group: "", timezone: "Europe/Kaliningrad", display: "Kaliningrad" },
      { group: "", timezone: "Europe/Kiev", display: "Kiev" },
      { group: "", timezone: "Europe/Kirov", display: "Kirov" },
      { group: "", timezone: "Europe/Lisbon", display: "Lisbon" },
      { group: "", timezone: "Europe/Ljubljana", display: "Ljubljana" },
      { group: "", timezone: "Europe/London", display: "London" },
      { group: "", timezone: "Europe/Luxembourg", display: "Luxembourg" },
      { group: "", timezone: "Europe/Madrid", display: "Madrid" },
      { group: "", timezone: "Europe/Malta", display: "Malta" },
      { group: "", timezone: "Europe/Mariehamn", display: "Mariehamn" },
      { group: "", timezone: "Europe/Minsk", display: "Minsk" },
      { group: "", timezone: "Europe/Monaco", display: "Monaco" },
      { group: "", timezone: "Europe/Moscow", display: "Moscow" },
      { group: "", timezone: "Europe/Oslo", display: "Oslo" },
      { group: "", timezone: "Europe/Paris", display: "Paris" },
      { group: "", timezone: "Europe/Podgorica", display: "Podgorica" },
      { group: "", timezone: "Europe/Prague", display: "Prague" },
      { group: "", timezone: "Europe/Riga", display: "Riga" },
      { group: "", timezone: "Europe/Rome", display: "Rome" },
      { group: "", timezone: "Europe/Samara", display: "Samara" },
      { group: "", timezone: "Europe/San_Marino", display: "San Marino" },
      { group: "", timezone: "Europe/Sarajevo", display: "Sarajevo" },
      { group: "", timezone: "Europe/Saratov", display: "Saratov" },
      { group: "", timezone: "Europe/Simferopol", display: "Simferopol" },
      { group: "", timezone: "Europe/Skopje", display: "Skopje" },
      { group: "", timezone: "Europe/Sofia", display: "Sofia" },
      { group: "", timezone: "Europe/Stockholm", display: "Stockholm" },
      { group: "", timezone: "Europe/Tallinn", display: "Tallinn" },
      { group: "", timezone: "Europe/Tirane", display: "Tirane" },
      { group: "", timezone: "Europe/Ulyanovsk", display: "Ulyanovsk" },
      { group: "", timezone: "Europe/Uzhgorod", display: "Uzhgorod" },
      { group: "", timezone: "Europe/Vaduz", display: "Vaduz" },
      { group: "", timezone: "Europe/Vatican", display: "Vatican" },
      { group: "", timezone: "Europe/Vienna", display: "Vienna" },
      { group: "", timezone: "Europe/Vilnius", display: "Vilnius" },
      { group: "", timezone: "Europe/Volgograd", display: "Volgograd" },
      { group: "", timezone: "Europe/Warsaw", display: "Warsaw" },
      { group: "", timezone: "Europe/Zagreb", display: "Zagreb" },
      { group: "", timezone: "Europe/Zaporozhye", display: "Zaporozhye" },
      { group: "", timezone: "Europe/Zurich", display: "Zurich" }
    ];
  };

  _p.timeZoneData.indian = function () {
    return [
      { group: "", timezone: "Indian/Antananarivo", display: "Antananarivo" },
      { group: "", timezone: "Indian/Chagos", display: "Chagos" },
      { group: "", timezone: "Indian/Christmas", display: "Christmas" },
      { group: "", timezone: "Indian/Cocos", display: "Cocos" },
      { group: "", timezone: "Indian/Comoro", display: "Comoro" },
      { group: "", timezone: "Indian/Kerguelen", display: "Kerguelen" },
      { group: "", timezone: "Indian/Mahe", display: "Mahe" },
      { group: "", timezone: "Indian/Maldives", display: "Maldives" },
      { group: "", timezone: "Indian/Mauritius", display: "Mauritius" },
      { group: "", timezone: "Indian/Mayotte", display: "Mayotte" },
      { group: "", timezone: "Indian/Reunion", display: "Reunion" }
    ];
  };

  _p.timeZoneData.pacific = function () {
    return [
      { group: "", timezone: "Pacific/Apia", display: "Apia" },
      { group: "", timezone: "Pacific/Auckland", display: "Auckland" },
      { group: "", timezone: "Pacific/Bougainville", display: "Bougainville" },
      { group: "", timezone: "Pacific/Chatham", display: "Chatham" },
      { group: "", timezone: "Pacific/Chuuk", display: "Chuuk" },
      { group: "", timezone: "Pacific/Easter", display: "Easter" },
      { group: "", timezone: "Pacific/Efate", display: "Efate" },
      { group: "", timezone: "Pacific/Enderbury", display: "Enderbury" },
      { group: "", timezone: "Pacific/Fakaofo", display: "Fakaofo" },
      { group: "", timezone: "Pacific/Fiji", display: "Fiji" },
      { group: "", timezone: "Pacific/Funafuti", display: "Funafuti" },
      { group: "", timezone: "Pacific/Galapagos", display: "Galapagos" },
      { group: "", timezone: "Pacific/Gambier", display: "Gambier" },
      { group: "", timezone: "Pacific/Guadalcanal", display: "Guadalcanal" },
      { group: "", timezone: "Pacific/Guam", display: "Guam" },
      { group: "", timezone: "Pacific/Honolulu", display: "Honolulu" },
      { group: "", timezone: "Pacific/Kiritimati", display: "Kiritimati" },
      { group: "", timezone: "Pacific/Kosrae", display: "Kosrae" },
      { group: "", timezone: "Pacific/Kwajalein", display: "Kwajalein" },
      { group: "", timezone: "Pacific/Majuro", display: "Majuro" },
      { group: "", timezone: "Pacific/Marquesas", display: "Marquesas" },
      { group: "", timezone: "Pacific/Midway", display: "Midway" },
      { group: "", timezone: "Pacific/Nauru", display: "Nauru" },
      { group: "", timezone: "Pacific/Niue", display: "Niue" },
      { group: "", timezone: "Pacific/Norfolk", display: "Norfolk" },
      { group: "", timezone: "Pacific/Noumea", display: "Noumea" },
      { group: "", timezone: "Pacific/Pago_Pago", display: "Pago Pago" },
      { group: "", timezone: "Pacific/Palau", display: "Palau" },
      { group: "", timezone: "Pacific/Pitcairn", display: "Pitcairn" },
      { group: "", timezone: "Pacific/Pohnpei", display: "Pohnpei" },
      { group: "", timezone: "Pacific/Port_Moresby", display: "Port Moresby" },
      { group: "", timezone: "Pacific/Rarotonga", display: "Rarotonga" },
      { group: "", timezone: "Pacific/Saipan", display: "Saipan" },
      { group: "", timezone: "Pacific/Tahiti", display: "Tahiti" },
      { group: "", timezone: "Pacific/Tarawa", display: "Tarawa" },
      { group: "", timezone: "Pacific/Tongatapu", display: "Tongatapu" },
      { group: "", timezone: "Pacific/Wake", display: "Wake" },
      { group: "", timezone: "Pacific/Wallis", display: "Wallis" }
    ];
  };

  return p;
})();
//$(document).ready(function () { appts_tz.init(); });
