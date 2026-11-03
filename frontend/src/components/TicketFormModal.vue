<script setup>
import { reactive, watch } from 'vue'
import { ASSIGNEES, PRIORITY_LABELS, STATUS_LABELS } from '../api/client'

const props = defineProps({
  open: { type: Boolean, default: false },
  mode: { type: String, default: 'create' },
  ticket: { type: Object, default: null },
  busy: { type: Boolean, default: false }
})

const emit = defineEmits(['close', 'submit'])

const form = reactive({
  title: '',
  description: '',
  status: 0,
  priority: 1,
  customerName: '',
  customerEmail: '',
  assignee: ''
})

watch(
  () => [props.open, props.ticket, props.mode],
  () => {
    if (!props.open) return
    if (props.mode === 'edit' && props.ticket) {
      form.title = props.ticket.title
      form.description = props.ticket.description
      form.status = props.ticket.status
      form.priority = props.ticket.priority
      form.customerName = props.ticket.customerName
      form.customerEmail = props.ticket.customerEmail
      form.assignee = props.ticket.assignee || ''
    } else {
      form.title = ''
      form.description = ''
      form.status = 0
      form.priority = 1
      form.customerName = ''
      form.customerEmail = ''
      form.assignee = ''
    }
  },
  { immediate: true }
)

function onSubmit() {
  emit('submit', { ...form, assignee: form.assignee || null })
}
</script>

<template>
  <Teleport to="body">
    <div
      v-if="open"
      class="fixed inset-0 z-50 flex items-start justify-center overflow-y-auto bg-ink-950/40 p-4 backdrop-blur-[2px] sm:items-center"
      @click.self="$emit('close')"
    >
      <form class="panel my-4 w-full max-w-xl p-6" @submit.prevent="onSubmit">
        <div class="flex items-start justify-between gap-4">
          <div>
            <p class="text-xs font-semibold uppercase tracking-[0.16em] text-tide-700">
              {{ mode === 'edit' ? 'Edit ticket' : 'New ticket' }}
            </p>
            <h3 class="mt-1 font-display text-2xl text-ink-950">
              {{ mode === 'edit' ? 'Update support request' : 'Log a customer issue' }}
            </h3>
          </div>
          <button type="button" class="btn-secondary px-3" @click="$emit('close')">Close</button>
        </div>

        <div class="mt-6 grid gap-4">
          <div>
            <label class="label" for="title">Title</label>
            <input id="title" v-model="form.title" class="field" required maxlength="200" />
          </div>
          <div>
            <label class="label" for="description">Description</label>
            <textarea
              id="description"
              v-model="form.description"
              class="field min-h-[110px] resize-y"
              required
              maxlength="4000"
            />
          </div>
          <div class="grid gap-4 sm:grid-cols-2">
            <div v-if="mode === 'edit'">
              <label class="label" for="status">Status</label>
              <select id="status" v-model.number="form.status" class="field">
                <option v-for="(label, value) in STATUS_LABELS" :key="value" :value="Number(value)">
                  {{ label }}
                </option>
              </select>
            </div>
            <div>
              <label class="label" for="priority">Priority</label>
              <select id="priority" v-model.number="form.priority" class="field">
                <option v-for="(label, value) in PRIORITY_LABELS" :key="value" :value="Number(value)">
                  {{ label }}
                </option>
              </select>
            </div>
            <div>
              <label class="label" for="assignee">Assignee</label>
              <select id="assignee" v-model="form.assignee" class="field">
                <option value="">Unassigned</option>
                <option v-for="name in ASSIGNEES" :key="name" :value="name">{{ name }}</option>
              </select>
            </div>
          </div>
          <div class="grid gap-4 sm:grid-cols-2">
            <div>
              <label class="label" for="customerName">Customer name</label>
              <input id="customerName" v-model="form.customerName" class="field" required maxlength="120" />
            </div>
            <div>
              <label class="label" for="customerEmail">Customer email</label>
              <input
                id="customerEmail"
                v-model="form.customerEmail"
                type="email"
                class="field"
                required
                maxlength="200"
              />
            </div>
          </div>
        </div>

        <div class="mt-6 flex justify-end gap-2">
          <button type="button" class="btn-secondary" :disabled="busy" @click="$emit('close')">
            Cancel
          </button>
          <button type="submit" class="btn-primary" :disabled="busy">
            {{ busy ? 'Saving…' : mode === 'edit' ? 'Save changes' : 'Create ticket' }}
          </button>
        </div>
      </form>
    </div>
  </Teleport>
</template>
