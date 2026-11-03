<script setup>
import { ASSIGNEES, PRIORITY_LABELS, STATUS_LABELS } from '../api/client'

const props = defineProps({
  modelValue: { type: Object, required: true }
})

const emit = defineEmits(['update:modelValue', 'clear'])

function set(key, value) {
  emit('update:modelValue', { ...props.modelValue, [key]: value })
}

function toggleSort(field) {
  if (props.modelValue.sortBy === field) {
    set('sortDir', props.modelValue.sortDir === 'asc' ? 'desc' : 'asc')
    return
  }

  emit('update:modelValue', {
    ...props.modelValue,
    sortBy: field,
    sortDir: 'desc'
  })
}

const sortOptions = [
  { key: 'createdAt', label: 'Created' },
  { key: 'priority', label: 'Priority' },
  { key: 'status', label: 'Status' },
  { key: 'updatedAt', label: 'Updated' }
]
</script>

<template>
  <div class="panel p-4 sm:p-5">
    <div class="flex flex-wrap items-end justify-between gap-3">
      <div>
        <h3 class="font-display text-xl text-ink-950">Ticket queue</h3>
        <p class="text-sm text-ink-500">Filter, sort, and scan the board without leaving the list.</p>
      </div>
      <button type="button" class="btn-secondary" @click="$emit('clear')">Reset filters</button>
    </div>

    <div class="mt-4 grid gap-3 md:grid-cols-2 xl:grid-cols-5">
      <div class="xl:col-span-2">
        <label class="label" for="search">Search</label>
        <input
          id="search"
          class="field"
          placeholder="Title, description, customer…"
          :value="modelValue.search"
          @input="set('search', $event.target.value)"
        />
      </div>
      <div>
        <label class="label" for="status">Status</label>
        <select
          id="status"
          class="field"
          :value="modelValue.status"
          @change="set('status', $event.target.value === '' ? '' : Number($event.target.value))"
        >
          <option value="">All statuses</option>
          <option v-for="(label, value) in STATUS_LABELS" :key="value" :value="value">{{ label }}</option>
        </select>
      </div>
      <div>
        <label class="label" for="priority">Priority</label>
        <select
          id="priority"
          class="field"
          :value="modelValue.priority"
          @change="set('priority', $event.target.value === '' ? '' : Number($event.target.value))"
        >
          <option value="">All priorities</option>
          <option v-for="(label, value) in PRIORITY_LABELS" :key="value" :value="value">{{ label }}</option>
        </select>
      </div>
      <div>
        <label class="label" for="assignee">Assignee</label>
        <select
          id="assignee"
          class="field"
          :value="modelValue.assignee"
          @change="set('assignee', $event.target.value)"
        >
          <option value="">Anyone</option>
          <option v-for="name in ASSIGNEES" :key="name" :value="name">{{ name }}</option>
        </select>
      </div>
    </div>

    <div class="mt-4 flex flex-wrap items-center gap-2">
      <span class="text-xs font-semibold uppercase tracking-wide text-ink-500">Sort</span>
      <button
        v-for="opt in sortOptions"
        :key="opt.key"
        type="button"
        class="rounded-full border px-3 py-1 text-xs font-semibold transition"
        :class="
          modelValue.sortBy === opt.key
            ? 'border-tide-600 bg-tide-600 text-white'
            : 'border-ink-200 bg-white text-ink-700 hover:bg-ink-50'
        "
        @click="toggleSort(opt.key)"
      >
        {{ opt.label }}
        <span v-if="modelValue.sortBy === opt.key" class="ml-1 opacity-80">
          {{ modelValue.sortDir === 'asc' ? '↑' : '↓' }}
        </span>
      </button>
    </div>
  </div>
</template>
