/**
 * computer3d.js — Workstation Power Control & Interactions
 * IT Helpdesk — Trương Tuấn Vĩnh — 2026
 */

(function () {
    'use strict';

    function initPowerInteraction() {
        var robotScene = document.getElementById('robotScene');
        var mainPowerToggle = document.getElementById('mainPowerToggle');

        if (!robotScene) return;
        var svg = robotScene.querySelector('svg');

        // Master state variable: starts as false (meaning system is ON by default)
        var isForceOff = false;

        // Sync startup state based on classes present in HTML on load
        if (robotScene.classList.contains('power-off')) {
            isForceOff = true;
            if (svg && svg.pauseAnimations) {
                svg.pauseAnimations();
            }
        }

        function setPowerState(turnOff) {
            if (turnOff) {
                robotScene.classList.add('power-off');
                if (mainPowerToggle) mainPowerToggle.classList.add('power-off-active');
                if (svg && svg.pauseAnimations) svg.pauseAnimations();
            } else {
                robotScene.classList.remove('power-off');
                if (mainPowerToggle) mainPowerToggle.classList.remove('power-off-active');
                if (svg && svg.unpauseAnimations) svg.unpauseAnimations();
            }
        }

        // Click master toggle switch
        function handleToggleClick() {
            isForceOff = !isForceOff;
            setPowerState(isForceOff);
        }

        if (mainPowerToggle) {
            mainPowerToggle.addEventListener('click', handleToggleClick);
        }


    }

    if (document.readyState === 'loading') {
        window.addEventListener('DOMContentLoaded', initPowerInteraction);
    } else {
        initPowerInteraction();
    }
})();
