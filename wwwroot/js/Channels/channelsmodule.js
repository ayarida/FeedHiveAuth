
    var addChannel;
    var data = {
        selectAllChannels: false,
        wizard: '',
        credentials: '',
        id: '',
        errorMsg: '',
        searchQuery: '',
        addedChannels: [],
        submitting: false
    };

    function AddChannel() {
        return new Vue({
            el: '.page-content',
            data: data,
            computed: {
                showProfile: function () {
                    return validateNetworkAccount(this.network, 'profile');
                },
                showPage: function () {
                    return validateNetworkAccount(this.network, 'page');
                },
                showGroup: function () {
                    return validateNetworkAccount(this.network, 'group');
                },
                showBusiness: function () {
                    return validateNetworkAccount(this.network, 'business');
                },
                showAdManager: function () {
                    return validateNetworkAccount(this.network, 'admanager');
                },
                selectedChannels: function () {
                    var self = this;
                    return self.newChannels.filter(channel => {
                        return self.selectedChannelsId.includes(channel.NetworkId);
                    });
                },
                filteredChannels: function () {
                    var self = this;
                    if (!self.searchQuery) {
                        return self.newChannels;
                    }

                    var channels = self.newChannels.filter(channel => {
                        var matchUsername = channel.Code && channel.Code.toLowerCase().includes(self.searchQuery);
                        var matchOriginalName = channel.OriginalName && channel.OriginalName.toLowerCase().includes(self.searchQuery)
                        return matchUsername || matchOriginalName;
                    });
                    return channels;
                }
            },
            watch: {
                errorMsg: function (val) {
                    $.gritter.add({
                        time: 5000,
                        text: '<i class="icon-bell-alt icon-animated-bell"></i> ' + val,
                        class_name: 'gritter-center gritter-warning '
                    });
                }
            },
            methods: {
                updateNetwork: function (type, enabled) {
                    console.log("updatenetwork");
                    if (!enabled) {
                        this.errorMsg = type + " network is not enabled. <br> Please contact support to enable it.";
                        return;
                    }
                    this.network = type;
                    setWizardStep(2);
                },
                authorize: function (type) {

                    this.accountType = type;
                    if (this.network === 'telegram' || this.network === 'firebase')
                        setWizardStep(3);
                    else
                        this.signIn();
                },
                channelIsOld: function (networkId) {
                    var oldChannel = this.channels.find(channel => channel.NetworkId === networkId && channel.Status === 90);
                    return !!oldChannel;
                },
                updateSelectedChannels: function (event) {
                    var self = this;
                    var selectAll = event.target.checked;
                    if (!selectAll) {
                        var selectedChannels = self.newChannels.filter(newChannel => {
                            return !!data.channels.find(channel => channel.NetworkId === newChannel.NetworkId && channel.Status === 90);
                        });

                        self.selectedChannelsId = $.map(selectedChannels, function (channel) {
                            return channel.NetworkId;
                        });
                        return;
                    }

                    self.selectedChannelsId = $.map(self.newChannels, function (channel) {
                        return channel.NetworkId;
                    });
                },
                signIn: function () {
                    var self = this;
                    self.submitting = true;
                    if ((self.network === 'telegram' && !self.id) || (self.network === 'firebase' && !self.credentials)) return;
                    $.ajax('/Authorization/OAuthFlow', {
                        method: 'POST',
                        data: { network: self.network, account: self.accountType, id: self.id, credentials: self.credentials },
                        success: function (result) {
                            if (result.success && result.redirect)
                                window.location.assign(result.redirect);
                            else this.errorMsg = result.message;
                            self.submitting = false;
                        },
                        error: function (error) {
                            this.errorMsg = error;
                            self.submitting = false;
                        }
                    })
                },
                searchNewChannels: function (event) {
                    this.searchQuery = event.target.value.toLowerCase();
                },
                submitChannels: function () {
                    var self = this;
                    self.submitting = true;
                    $.ajax('/Channel/Save', {
                        method: 'POST',
                        data: { Items: self.selectedChannels },
                        success: function (result) {
                            if (result.success) {
                                self.addedChannels = result.channels;
                                setWizardStep(4);
                            }
                            else this.errorMsg = result.message;
                            self.submitting = false;
                        },
                        error: function (error) {
                            this.errorMsg = error;
                            self.submitting = false;
                        }
                    })
                }
            },
            created() {
                var self = this;
                $('[data-rel=tooltip]').tooltip();
                $.getScript('/Content/js/ace/wizard.min.js',
                    function () {
                        $('#add-channel-wizard-container').ace_wizard();
                        self.wizard = $('#add-channel-wizard-container').data('fu.wizard');
                        if (self.newChannels.length === 1) {
                            self.addedChannels = self.newChannels;
                            setWizardStep(4);
                        }
                        else if (self.newChannels.length > 1) setWizardStep(3);
                        else if (self.network && self.accountType && (self.network === 'telegram' || self.network === 'firebase')) setWizardStep(3);
                        else if (self.network && self.accountType && validateNetworkAccount(self.network, self.accountType)) self.signIn();
                        else if (self.network) setWizardStep(2);
                        showPageNotification($('#add_channel'));
                    });
            }
        });
    }

    function init() {
        data.network = createChannelData.Type;
        data.accountType = createChannelData.Account;
        data.channels = createChannelData.AllChannels;
        data.newChannels = createChannelData.NewChannels;
        var selectedChannels = data.newChannels.filter(newChannel => {
            return !!data.channels.find(channel => channel.NetworkId === newChannel.NetworkId && channel.Status === 90);
        });
        data.selectedChannelsId = $.map(selectedChannels, function (channel) {
            return channel.NetworkId;
        });
        addChannel = new AddChannel();
    }

    function setWizardStep(step) {
        if (!addChannel.wizard) return;
        addChannel.wizard.currentStep = step;
        addChannel.wizard.setState();
    }

    function validateNetworkAccount(network, account) {
        network = network.toLowerCase();
        switch (account.toLowerCase()) {
            case 'profile': return network !== 'telegram';
            case 'page': return network === 'facebook' || network === 'telegram';
            case 'business': return network === 'whatsapp' || network === 'instagram';
            case 'admanager': return network === 'facebook';
            case 'group': return network === 'facebook';
        }
    }

