import { createRouter, createWebHistory } from 'vue-router'
import DashboardView from '../views/DashboardView.vue'
import TicketDetailView from '../views/TicketDetailView.vue'

const router = createRouter({
  history: createWebHistory(),
  routes: [
    { path: '/', name: 'dashboard', component: DashboardView },
    { path: '/tickets/:id', name: 'ticket-detail', component: TicketDetailView, props: true }
  ],
  scrollBehavior() {
    return { top: 0 }
  }
})

export default router
