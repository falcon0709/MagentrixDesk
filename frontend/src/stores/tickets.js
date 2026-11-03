import { defineStore } from 'pinia'
import { api } from '../api/client'

export const useTicketsStore = defineStore('tickets', {
  state: () => ({
    tickets: [],
    stats: null,
    currentTicket: null,
    loadingList: false,
    loadingDetail: false,
    saving: false,
    error: null,
    filters: {
      search: '',
      status: '',
      priority: '',
      assignee: '',
      sortBy: 'createdAt',
      sortDir: 'desc',
      needsAttention: false
    }
  }),

  actions: {
    async fetchTickets() {
      this.loadingList = true
      this.error = null
      try {
        const params = {
          search: this.filters.search || undefined,
          status: this.filters.status === '' ? undefined : this.filters.status,
          priority: this.filters.priority === '' ? undefined : this.filters.priority,
          assignee: this.filters.assignee || undefined,
          sortBy: this.filters.sortBy,
          sortDir: this.filters.sortDir,
          needsAttention: this.filters.needsAttention || undefined
        }
        this.tickets = await api.getTickets(params)
      } catch (err) {
        this.error = err.message
      } finally {
        this.loadingList = false
      }
    },

    async fetchStats() {
      try {
        this.stats = await api.getStats()
      } catch (err) {
        this.error = err.message
      }
    },

    async fetchTicket(id) {
      this.loadingDetail = true
      this.error = null
      try {
        this.currentTicket = await api.getTicket(id)
      } catch (err) {
        this.error = err.message
        this.currentTicket = null
      } finally {
        this.loadingDetail = false
      }
    },

    async createTicket(payload) {
      this.saving = true
      try {
        const ticket = await api.createTicket(payload)
        await Promise.all([this.fetchTickets(), this.fetchStats()])
        return ticket
      } finally {
        this.saving = false
      }
    },

    async updateTicket(id, payload) {
      this.saving = true
      try {
        const ticket = await api.updateTicket(id, payload)
        this.currentTicket = ticket
        await Promise.all([this.fetchTickets(), this.fetchStats()])
        return ticket
      } finally {
        this.saving = false
      }
    },

    async deleteTicket(id) {
      this.saving = true
      try {
        await api.deleteTicket(id)
        if (this.currentTicket?.id === id) {
          this.currentTicket = null
        }
        await Promise.all([this.fetchTickets(), this.fetchStats()])
      } finally {
        this.saving = false
      }
    },

    async addComment(id, payload) {
      this.saving = true
      try {
        await api.addComment(id, payload)
        await this.fetchTicket(id)
        await this.fetchTickets()
      } finally {
        this.saving = false
      }
    },

    setFilters(partial) {
      this.filters = { ...this.filters, ...partial }
    },

    clearFilters() {
      this.filters = {
        search: '',
        status: '',
        priority: '',
        assignee: '',
        sortBy: 'createdAt',
        sortDir: 'desc',
        needsAttention: false
      }
    }
  }
})
