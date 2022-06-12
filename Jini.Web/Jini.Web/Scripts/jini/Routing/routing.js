var routing = function ($stateProvider, $urlRouterProvider) {

    $stateProvider.state('start',
    {
        url: '/',
        ncyBreadcrumb: {
            label: 'Digitale produkter'
        }
    });


    $stateProvider.state('history',
    {
        url: '/History',
        views: {
            'Main': {
                templateUrl: '/Wizard/History',
            }

        }
    });

    $stateProvider.state('history.draft',
    {
        parent: 'history',
        url: '/History/Draft',
        views: {
            'VersionMessage': {
                templateUrl: '/Wizard/DraftPanel',
            }

        }
    });

    $stateProvider.state('history.new',
    {
        parent: 'history',
        url: '/History/New',
        views: {
            'VersionMessage': {
                templateUrl: '/Wizard/NewPanel',
            }

        }
    });


    $stateProvider.state('create',
    {
        url: '/Create',
        views: {
            'Main': {
                templateUrl: '/Wizard/Create'
            },
            'ProgressBar': {
                templateUrl: "/Wizard/ProgressBar_Create"
            }

        }
    });

    $stateProvider.state('dashboard',
        {
            url: '/Dashboard',
            views: {
                'Main': {
                    templateUrl: '/Wizard/Dashboard'
                }

            }
        });

    $stateProvider.state('config',
    {
        url: '/Config',
        views: {
            'Main': {
                templateUrl: '/Wizard/Config'
            },
            'ProgressBar': {
                templateUrl: "/Wizard/ProgressBar_Config"
            }
        }
    });

    $stateProvider.state('trial',
    {
        url: '/Trial',
        views: {
            'Main': {
                templateUrl: '/Wizard/Trial',
            },
            'ProgressBar': {
                templateUrl: "/Wizard/ProgressBar_Trial",
            }
        }
    });

    $stateProvider.state('approve',
    {
        url: '/Approve',
        views: {
            'Main': {
                templateUrl: '/Wizard/Approve'
            },
            'ProgressBar': {
                templateUrl: "/Wizard/ProgressBar_Approve"
            }
        }
    });

    
    $stateProvider.state('published',
    {
        url: '/Published',
        views: {
            'Main': {
                templateUrl: '/Wizard/Publish'
            },
            'ProgressBar': {
                template: ""
            }
        }
    });

    $urlRouterProvider.otherwise('/');

}

routing.$inject = ['$stateProvider', '$urlRouterProvider'];